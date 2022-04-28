using System.Data;
using Microsoft.Extensions.Logging;
using DotNetCommon.Extensions;
using DotNetCommon.Helpers;
using EIA.Services.Clients;
using EIA.Domain.Model;
using PiServices;
using PiModel;
using UnitsNet;
using UIowaBuildingsModel;
using UIowaBuildingsModel.ConversionMethods;

namespace UIowaBuildingsServices
{
    public class ReportingService
    {
        private readonly PiHttpClient _piClient;
        private readonly EiaClient _eiaClient;
        private readonly ILogger<ReportingService> _logger;

        public ReportingService(PiHttpClient piClient, EiaClient eiaClient, ILogger<ReportingService> logger)
        {
            _piClient = piClient;
            _eiaClient = eiaClient;
            _logger = logger;
        }

        public async Task<CampusSnapshot> GenerateCampusSnapshot(HourlyEmissionsReportParameters parameters)
        {
            //UIowa is obviously in central time... So for the sake of this report we can assume that our intention is to render data in central time,
            //but if we were looking at buildings in multiple time zones obviously this would need to be changed
            DateTimeOffset startDateTime = new DateTimeOffset(parameters.StartDateInLocalTime.Date, TimeZones.GetCentralTimeOffset(parameters.StartDateInLocalTime.Date)); //Report convention, start at 12AM (first hour) of the given startDate
            DateTimeOffset endDateTime = new DateTimeOffset(parameters.EndDateInLocalTime.AddDays(1).AddHours(-1), TimeZones.GetCentralTimeOffset(parameters.StartDateInLocalTime.Date)); //Report convention, end at 11PM (last hour) of the given endDate

            //CAMPUS RESOURCE INFORMATION
            CampusEnergyResourceManager energyResourceManager = new CampusEnergyResourceManager(startDateTime, endDateTime);

            //Add data about how much energy was purchased vs how much energy was generated on campus
            await AddCampusPurchasedToGeneratedRatioInformation(energyResourceManager);

            //Add the MISO data for every grid source for the given timeframe
            await AddGridOperatorElectricSources(energyResourceManager, parameters.GridStrategy);

            //Get information about on campus power production
            PowerPlantDataMapper powerPlantMapper = await PopulatePowerPlantMapper(energyResourceManager.StartDateTime, energyResourceManager.EndDateTime);
            energyResourceManager.AddPowerPlantData(powerPlantMapper);

            //BUILDING RESOURCE USAGE

            //For every asset, get the electric, steam, and chilled water usage
            ICollection<BuildingUsageMapper> buildingMappers = new List<BuildingUsageMapper>();
            foreach (string buildingLink in parameters.AssetLinks)
            {
                BuildingUsageMapper buildingUsageMapper = await PopulateBuildingData(startDateTime, endDateTime, buildingLink);
                buildingMappers.Add(buildingUsageMapper);
            }

            List<BuildingUsageSummary> usageSummaries = buildingMappers.Select(x => x.PackageDataIntoBuildingUsageSummary()).ToList();
            energyResourceManager.PopulateCo2EmissionsData(usageSummaries);

            CampusSnapshot snapshot = new CampusSnapshot
            {
                Start = startDateTime,
                End = endDateTime,
                ElectricGridStrategy = parameters.GridStrategy.ToDescription(),
                BuildingUsageSummaries = usageSummaries,
                EnergyResources = energyResourceManager.EnumerateHoursDetails(),
                CampusDataSources = powerPlantMapper
            };

            return snapshot;
        }

        //CAMPUS BUILDING USAGE
        private async Task<BuildingUsageMapper> PopulateBuildingData(DateTimeOffset startDateTime, DateTimeOffset endDateTime, string buildingLink)
        {
            Asset asset = await _piClient.GetByDirectLink<Asset>(buildingLink);
            await _piClient.LoadAssetValueList(asset);

            BuildingUsageMapper buildingUsageMapper = new BuildingUsageMapper();
            buildingUsageMapper.BuildingName = asset.Name;

            //Electric Usage
            AssetValue hourlyElectric = asset.ChildValues.Where(x => x.Name.CapsAndTrim() == "EL POWER HOURLY AVG").First();
            asset.ChildValues.Remove(hourlyElectric);
            hourlyElectric = await _piClient.LoadAssetValueDetail(hourlyElectric);
            asset.ChildValues.Add(hourlyElectric);
            if (hourlyElectric.DefaultUnitsName == "kilowatt")
            {
                //Same story about data being in kw but converting to kwh which is assuming that the power demand remained constant for the whole hour
                buildingUsageMapper.DataPointToElectricUsageFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) => { return Energy.FromKilowattHours(dataPoint.Value); });
            }
            else throw new NotImplementedException($"Function to convert data points with units {hourlyElectric.DefaultUnitsName} to electric usage has not been implemented.");

            await _piClient.LoadInterpolatedValues(hourlyElectric, startDateTime, endDateTime);
            buildingUsageMapper.ElectricUsageDataPoints = hourlyElectric.InterpolatedDataPoints;

            //Steam Usage
            AssetValue hourlySteam = asset.ChildValues.Where(x => x.Name.CapsAndTrim() == "ST FLOW HOURLY AVG").FirstOrDefault();
            if (hourlySteam != null) //Not all buildings use steam heating (I.E. Buildings not connected to main system)
            {
                asset.ChildValues.Remove(hourlySteam);
                hourlySteam = await _piClient.LoadAssetValueDetail(hourlySteam);
                asset.ChildValues.Add(hourlySteam);
                if (hourlySteam.DefaultUnitsName == "thousand pound per hour")
                {
                    buildingUsageMapper.DataPointToSteamUsageAsMassFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { return Mass.FromKilopounds(dataPoint.Value); });
                    buildingUsageMapper.DataPointToSteamUsageAsEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                    {
                        double btusPerPoundOfSteam = 1192; //It depends on whether it’s 20 psi or 155 psi.  20 PSI – 1192 BTU/LB, 155 PSI – 1224 BTU/LB. TODO: Find the PSI
                        double totalBtus = dataPoint.Value * 1000 * btusPerPoundOfSteam;
                        return Energy.FromBritishThermalUnits(totalBtus);
                    });
                }
                else throw new NotImplementedException($"Function to convert data points with units {hourlySteam.DefaultUnitsName} to steam usage has not been implemented.");

                await _piClient.LoadInterpolatedValues(hourlySteam, startDateTime, endDateTime);
                buildingUsageMapper.SteamUsageDataPoints = hourlySteam.InterpolatedDataPoints;
            }

            //Chilled Water Usage
            AssetValue hourlyChilledWater = asset.ChildValues.Where(x => x.Name.CapsAndTrim() == "CW FLOW HOURLY AVG").FirstOrDefault();
            if (hourlyChilledWater != null) //Not all buildings use chilled water cooling (I.E. Buildings not connected to main system)
            {
                asset.ChildValues.Remove(hourlyChilledWater);
                hourlyChilledWater = await _piClient.LoadAssetValueDetail(hourlyChilledWater);
                asset.ChildValues.Add(hourlyChilledWater);
                if (hourlyChilledWater.DefaultUnitsName == "US gallon per minute")
                {
                    buildingUsageMapper.DataPointToChilledWaterVolumeFunction = new Func<InterpolatedDataPoint, Volume>((dataPoint) => { return Volume.FromUsGallons(dataPoint.Value * 60); }); //Multiply by 60 cuz data is per minute
                }
                else throw new NotImplementedException($"Function to convert data points with units {hourlyChilledWater.DefaultUnitsName} to chilled water usage has not been implemented.");

                await _piClient.LoadInterpolatedValues(hourlyChilledWater, startDateTime, endDateTime);
                buildingUsageMapper.ChilledWaterUsageDataPoints = hourlyChilledWater.InterpolatedDataPoints;
            }

            return buildingUsageMapper;
        }

        //ELECTRIC RATIO
        private async Task AddCampusPurchasedToGeneratedRatioInformation(CampusEnergyResourceManager manager)
        {
            PiPoint gridPurchasedElectric = await _piClient.SearchPiPoint("PP_electric_purch");
            PiPoint campusGeneratedElectric = await _piClient.SearchPiPoint("PP_electric_gen");
            PiPoint campusTotalElectricUsage = await _piClient.SearchPiPoint("PP_electric_TCL");

            //NOTE The below is giving instanaeous load in each hour (based on the minute/seconds in the datetime). Currently selecting the halfway point in the hour but could sample the hour and average for more accurate results.
            await _piClient.LoadInterpolatedValues(gridPurchasedElectric, manager.StartDateTime.AddMinutes(30), manager.EndDateTime.AddMinutes(30));
            await _piClient.LoadInterpolatedValues(campusGeneratedElectric, manager.StartDateTime.AddMinutes(30), manager.EndDateTime.AddMinutes(30));
            await _piClient.LoadInterpolatedValues(campusTotalElectricUsage, manager.StartDateTime.AddMinutes(30), manager.EndDateTime.AddMinutes(30));

            //NOTE technically the data is in MWs, but am treating it as if it is MWHs. Essentially assuming the instaneous load was the same for a whole hour (lossy but ok for now)
            Func<InterpolatedDataPoint, Energy> pointToEnergyMapper = new Func<InterpolatedDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value); });

            DataPointsToEnergyMapper electricUsageMapper = new DataPointsToEnergyMapper
            {
                DataPoints = campusTotalElectricUsage.InterpolatedDataPoints,
                DataPointToEnergyFunction = pointToEnergyMapper
            };

            DataPointsToEnergyMapper electricGenerationMapper = new DataPointsToEnergyMapper
            {
                DataPoints = campusGeneratedElectric.InterpolatedDataPoints,
                DataPointToEnergyFunction = pointToEnergyMapper
            };

            manager.AddCampusElectricUsage(electricUsageMapper);
            manager.AddCampusElectricGeneration(electricGenerationMapper);
        }

        //CAMPUS
        public async Task<PowerPlantDataMapper> PopulatePowerPlantMapper(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            PowerPlantDataMapper powerPlantMapper = OnCampusPowerMapping.BuildPowerPlantMapper();

            await AddSteamProductionData(powerPlantMapper.SteamProductionMapper, startTime, endTime);
            await AddCogeneratedElectricData(powerPlantMapper.CogeneratedElectricMapper, startTime, endTime);
            await AddSteamOverheadData(powerPlantMapper.SteamOverheadMapper, startTime, endTime);
            await AddElectricOverheadData(powerPlantMapper.ElectricOverheadMapper, startTime, endTime);
            await AddChilledWaterPlantData(powerPlantMapper.ChilledWaterMapper, startTime, endTime);

            return powerPlantMapper;
        }

        private async Task AddSteamProductionData(SteamProductionMapper mapper, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            foreach (BoilerMapper boilerMapper in mapper.BoilerMappers)
            {
                foreach (BoilerInputMapper input in boilerMapper.BoilerInputMappers)
                {
                    PiPoint inputPiPoint = await _piClient.SearchPiPoint(input.InputTag);
                    await _piClient.LoadInterpolatedValues(inputPiPoint, startTime, endTime);
                    input.InputDataPoints = inputPiPoint.InterpolatedDataPoints;
                }

                PiPoint outputPiPoint = await _piClient.SearchPiPoint(boilerMapper.SteamOutputTag);
                await _piClient.LoadInterpolatedValues(outputPiPoint, startTime, endTime);
                boilerMapper.OutputDataPoints = outputPiPoint.InterpolatedDataPoints;
            }
        }

        private async Task AddCogeneratedElectricData(CogeneratedElectricMapper mapper, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            foreach (SteamToElectricTurbineMapper turbineMapper in mapper.TurbineProcessMappers)
            {
                foreach (TaggedDataSet dataSet in turbineMapper.InvolvedDataSets)
                {
                    PiPoint piPoint = await _piClient.SearchPiPoint(dataSet.Tag);
                    await _piClient.LoadInterpolatedValues(piPoint, startTime, endTime);
                    dataSet.DataPoints = piPoint.InterpolatedDataPoints;
                }
            }
        }

        private async Task AddSteamOverheadData(SteamOverheadMapper mapper, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            foreach (SteamOverheadFactorMapper factorMapper in mapper.FactorMappers)
            {
                PiPoint piPoint = await _piClient.SearchPiPoint(factorMapper.SteamMeterTag);
                await _piClient.LoadInterpolatedValues(piPoint, startTime, endTime);
                factorMapper.DataPoints = piPoint.InterpolatedDataPoints;
            }
        }

        private async Task AddElectricOverheadData(ElectricOverheadMapper mapper, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            foreach (ElectricOverheadFactorMapper factorMapper in mapper.FactorMappers)
            {
                if (!factorMapper.IsConstantValue)
                {
                    PiPoint electricMeterPiPoint = await _piClient.SearchPiPoint(factorMapper.ElectricMeterTag);
                    await _piClient.LoadInterpolatedValues(electricMeterPiPoint, startTime, endTime);
                    factorMapper.DataPoints = electricMeterPiPoint.InterpolatedDataPoints;
                }
            }
        }

        private async Task AddChilledWaterPlantData(ChilledWaterProductionMapper mapper, DateTimeOffset startTime, DateTimeOffset endTime)
        {
            foreach (ChilledWaterPlantMapper plantMapper in mapper.PlantMappers)
            {
                foreach (TaggedDataSet dataSet in plantMapper.ChilledWaterPlantInputDataSets)
                {
                    PiPoint piPoint = await _piClient.SearchPiPoint(dataSet.Tag);
                    await _piClient.LoadInterpolatedValues(piPoint, startTime, endTime);
                    dataSet.PopulateData(piPoint);
                }

                foreach (TaggedDataSet dataSet in plantMapper.ChilledWaterOutputDataSets)
                {
                    PiPoint piPoint = await _piClient.SearchPiPoint(dataSet.Tag);
                    await _piClient.LoadInterpolatedValues(piPoint, startTime, endTime, dataSet.FilterExpression);
                    dataSet.PopulateData(piPoint);
                }
            }
        }


        //GRID
        private async Task AddGridOperatorElectricSources(CampusEnergyResourceManager manager, ElectricGridStrategy gridStrategy)
        {
            if (gridStrategy == ElectricGridStrategy.MisoHourly)
            {
                IEnumerable<ElectricGridSourceMapper> gridSources = BuildMisoGridSourceMappers();

                ICollection<Series> allSourceSeries = new List<Series>(); //Need to hold on to these locally to backtrack petro data.
                bool backTrackMisoPetroData = false;

                foreach (ElectricGridSourceMapper source in gridSources)
                {//The sources are pulling everything in UTC time. There is an option to pull in local time but intentionally not using it
                 //because it is the local time of the grid operator not of this computing system. Note TimeZones.GetUtcOffset() gets an offset of 0.
                    Series sourceSeries = await _eiaClient.GetHourlySeriesByIdAsync(source.HourlySourceId, manager.StartDateTime, manager.EndDateTime, TimeZones.GetUtcOffset());

                    if (sourceSeries.DataPoints.Count != manager.TotalHours)
                    {
                        if (sourceSeries.DataPoints.Count < manager.TotalHours && sourceSeries.Id == "EBA.MISO-ALL.NG.OIL.H") //Been seeing issues with missing petro data. If all the other sources come through can back track it.
                        {
                            backTrackMisoPetroData = true;
                        }
                        else throw new Exception("The series by source query did not pull exactly as many data points as expected. Please investigate");
                    }
                    else
                    {
                        source.DataPoints = sourceSeries.DataPoints;
                        manager.AddGridOperatorElectricSource(source);
                        allSourceSeries.Add(sourceSeries);
                    }
                }

                if (backTrackMisoPetroData)
                {
                    _logger.LogInformation("Petro data missing. Attempting backtrack...");
                    Series misoTotal = await _eiaClient.GetHourlySeriesByIdAsync("EBA.MISO-ALL.NG.H", manager.StartDateTime, manager.EndDateTime, TimeZones.GetUtcOffset());
                    if (misoTotal.DataPoints.Count != manager.TotalHours) throw new Exception("The net generation series query did not pull exactly as many data points as expected. Please investigate");

                    ICollection<SeriesDataPoint> petroPoints = new List<SeriesDataPoint>();
                    foreach (SeriesDataPoint totalPoint in misoTotal.DataPoints)
                    {
                        double petroPointValue = totalPoint.Value.Value;
                        foreach (Series sourceSeries in allSourceSeries)
                        {
                            SeriesDataPoint sourcePoint = sourceSeries.DataPoints.Where(x => x.Timestamp.HourMatches(totalPoint.Timestamp)).First();
                            petroPointValue -= sourcePoint.Value.Value;
                        }

                        if (petroPointValue < 0) petroPointValue = 0;
                        petroPoints.Add(new SeriesDataPoint { Timestamp = totalPoint.Timestamp, Value = petroPointValue });
                    }

                    ElectricGridSourceMapper petroSource = gridSources.Where(x => x.HourlySourceId == "EBA.MISO-ALL.NG.OIL.H").First();
                    petroSource.DataPoints = petroPoints;
                    manager.AddGridOperatorElectricSource(petroSource);
                }
            }
            else if (gridStrategy == ElectricGridStrategy.MidAmericanAverageFuelMix)
            {
                IEnumerable<ElectricGridSourceMapper> mappers = BuildMidAmericanGridSourceMappers();
                foreach(ElectricGridSourceMapper sourceMapper in mappers)
                {
                    ICollection<SeriesDataPoint> pseudoPoints = new List<SeriesDataPoint>();
                    foreach(DateTimeOffset offset in manager.StartDateTime.EnumerateHoursUntil(manager.EndDateTime))
                    {
                        pseudoPoints.Add(new SeriesDataPoint { Timestamp = offset }); //Fake data, the function is just going to return a constant
                    }
                    sourceMapper.DataPoints = pseudoPoints;
                    manager.AddGridOperatorElectricSource(sourceMapper);
                }
            }
            else throw new ArgumentException("Grid strategy not known");
        }

        private IEnumerable<ElectricGridSourceMapper> BuildMisoGridSourceMappers()
        {
            ICollection<ElectricGridSourceMapper> sources = new List<ElectricGridSourceMapper>();

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Wind",
                HourlySourceId = "EBA.MISO-ALL.NG.WND.H",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((windEnergy) => { return Mass.Zero; }) //Wind has no carbon emissions, return 0
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Solar",
                HourlySourceId = "EBA.MISO-ALL.NG.SUN.H",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((solarEnergy) => { return Mass.Zero; }) //Solar has no carbon emissions, return 0
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Hydro",
                HourlySourceId = "EBA.MISO-ALL.NG.WAT.H",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((hydroEnergy) => { return Mass.Zero; }) //Hydro has no carbon emissions, return 0
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Coal",
                HourlySourceId = "EBA.MISO-ALL.NG.COL.H",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((coalEnergy) => { return Coal.ToCo2EmissionsFromElectricGenerated(coalEnergy); })
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Natural Gas",
                HourlySourceId = "EBA.MISO-ALL.NG.NG.H",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((naturalGasEnergy) => { return NaturalGas.ToCo2EmissionsFromElectricGenerated(naturalGasEnergy); })
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Nuclear",
                HourlySourceId = "EBA.MISO-ALL.NG.NUC.H",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((hydroEnergy) => { return Mass.Zero; }) //Nuclear has no carbon emissions, return 0
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Petro",
                HourlySourceId = "EBA.MISO-ALL.NG.OIL.H",
                KiloGramsOfCo2PerKwh = 0.9661517,
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((petroEnergy) => { return Petroleum.ToCo2EmissionsFromElectricGenerated(petroEnergy); })
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Other",
                HourlySourceId = "EBA.MISO-ALL.NG.OTH.H",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(dataPoint.Value.Value); }),
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((otherEnergy) => { return Other.ToCo2EmissionsFromElectricGenerated(otherEnergy); })
            });

            return sources;
        }

        private IEnumerable<ElectricGridSourceMapper> BuildMidAmericanGridSourceMappers()
        {//Generate using these averages: https://www.midamericanenergy.com/energy-mix
         //Base everything off of 10,000 mwh

            ICollection<ElectricGridSourceMapper> sources = new List<ElectricGridSourceMapper>();

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Wind",
                HourlySourceId = "MEC_AVERAGE_WIND",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(6200); }), //62%
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((windEnergy) => { return Mass.Zero; }) //Wind has no carbon emissions, return 0
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Coal",
                HourlySourceId = "MEC_AVERAGE_COAL",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(2300); }), //23%
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((coalEnergy) => { return Coal.ToCo2EmissionsFromElectricGenerated(coalEnergy); })
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Natural Gas",
                HourlySourceId = "MEC_AVERAGE_NATURAL_GAS",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(1100); }), //11%
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((naturalGasEnergy) => { return NaturalGas.ToCo2EmissionsFromElectricGenerated(naturalGasEnergy); })
            });

            sources.Add(new ElectricGridSourceMapper
            {
                SourceName = "Nuclear/Other",
                HourlySourceId = "MEC_AVERAGE_NUCLEAR_OR_OTHER",
                DataPointToEnergyFunction = new Func<SeriesDataPoint, Energy>((dataPoint) => { return Energy.FromMegawattHours(400); }), //4%
                EnergyToCo2EmissionsFunction = new Func<Energy, Mass>((other) => { return Other.ToCo2EmissionsFromElectricGenerated(other); })
            });

            return sources;
        }
    }
}