using UnitsNet;
using PiModel;
using UIowaBuildingsModel;
using UIowaBuildingsModel.ConversionMethods;

namespace UIowaBuildingsServices
{
    public static class OnCampusPowerMapping
    {
        public static PowerPlantDataMapper BuildPowerPlantMapper()
        {
            PowerPlantDataMapper powerPlantDataMapper = new PowerPlantDataMapper();

            powerPlantDataMapper.Name = "UIowa Power Plant";
            powerPlantDataMapper.SteamProductionMapper = BuildSteamProductionMapper();
            powerPlantDataMapper.CogeneratedElectricMapper = BuildCogeneratedElectricMapper();
            powerPlantDataMapper.ElectricOverheadMapper = BuildElectricOverheadMapper();
            powerPlantDataMapper.SteamOverheadMapper = BuildSteamOverheadMapper();
            powerPlantDataMapper.ChilledWaterMapper = BuildChilledWaterProductionMapper();

            return powerPlantDataMapper;
        }

        private static SteamProductionMapper BuildSteamProductionMapper()
        {
            SteamProductionMapper mapper = new SteamProductionMapper();
            mapper.Name = "UIowa Boilers";

            List<BoilerMapper> boilerMappers = new List<BoilerMapper>();

            boilerMappers.Add(new BoilerMapper
            {
                BoilerName = "Boiler 12",
                BoilerInputMappers = new List<BoilerInputMapper>()
                {
                    new BoilerInputMapper
                    {
                        InputName = "Boiler 12 Natural Gas Input",
                        InputTag = "PP_BLR12_FT_006_KSCFH",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { 
                            //boiler 12 input is in (standard) cubic kilo feet per hour and it represents a volume of natural gas
                            Volume naturalGas = Volume.FromKilocubicFeet(dataPoint.Value);
                            return NaturalGas.ToCo2Emissions(naturalGas);
                        })
                    }
                },
                SteamOutputTag = "PP_BLR12_MMBTU/HR_Steam-to-header",
                OuputToEnergyContentFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                {
                    //boiler 12 output is in MMBTUs per hour and it represents steam energy
                    Energy steamEnergy = Energy.FromMegabritishThermalUnits(dataPoint.Value);
                    return steamEnergy;
                })
            });

            boilerMappers.Add(new BoilerMapper
            {
                BoilerName = "Boiler 11",
                BoilerInputMappers = new List<BoilerInputMapper>
                {
                    new BoilerInputMapper
                    {
                        InputName = "Boiler 11 Coal Input",
                        InputTag = "PP_SF-WIT-6044A",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => {
                            //boiler 11 coal input is in kilo pounds per hour and it represents a mass of coal
                            Mass coal = Mass.FromKilopounds(dataPoint.Value);
                            return Coal.ToCo2Emissions(coal);
                        })
                    },
                    new BoilerInputMapper
                    {
                        InputName = "Boiler 11 Biomass Input",
                        InputTag = "PP_BIO_WEIGHT",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) =>
                        {
                            //boiler 11 biomass input is in kilo pounds per hour and it represents a mass of biomass
                            return Mass.Zero; //Let's assume biomass in carbon neutral for now
                        })
                    }
                },
                SteamOutputTag = "PP_BLR11_MMBTU/HR",
                OuputToEnergyContentFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                {
                    //boiler 11 output is in MMBTUs per hour and it represents steam energy
                    Energy steamEnergy = Energy.FromMegabritishThermalUnits(dataPoint.Value);
                    return steamEnergy;
                })
            });

            boilerMappers.Add(new BoilerMapper
            {
                BoilerName = "Boiler 10",
                BoilerInputMappers = new List<BoilerInputMapper>
                {
                    new BoilerInputMapper
                    {
                        InputName = "Boiler 10 Natural Gas Input",
                        InputTag = "PP_B10_FLT_235_FT",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { 
                            //boiler 10 natural gas input is in (standard) cubic feet per hour and it represents a volume of natural gas
                            Volume naturalGas = Volume.FromCubicFeet(dataPoint.Value);
                            return NaturalGas.ToCo2Emissions(naturalGas);
                        })
                    },
                    new BoilerInputMapper
                    {
                        InputName = "Boiler 10 Plastic Pellet And Agricultural Biproduct Input",
                        InputTag = "PP_B10_STOKER_AIR",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { 
                            //boiler 10 plastic pellet input is in kilo pounds per hour and it represents a mass of plastic pellets mixed with Agricultural bi product
                            Mass plasticAndAgriculturalBiproduct = Mass.FromKilopounds(dataPoint.Value);
                            return PlasticAgriculturalBiproductMix.ToCo2Emissions(plasticAndAgriculturalBiproduct);
                        })
                    }
                },
                SteamOutputTag = "PP_BLR10_MMBTU/HR",
                OuputToEnergyContentFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                {
                    //boiler 10 output is in MMBTUs per hour and it represents steam energy
                    Energy steamEnergy = Energy.FromMegabritishThermalUnits(dataPoint.Value);
                    return steamEnergy;
                })
            });

            boilerMappers.Add(new BoilerMapper
            {
                BoilerName = "Boiler 8",
                BoilerInputMappers = new List<BoilerInputMapper>()
                {
                    new BoilerInputMapper
                    {
                        InputName = "Boiler 8 Natural Gas Input",
                        InputTag = "PP_FI0821X",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { 
                            //boiler 8 input is in (standard) cubic kilo feet per hour and it represents a volume of natural gas
                            Volume naturalGas = Volume.FromKilocubicFeet(dataPoint.Value);
                            return NaturalGas.ToCo2Emissions(naturalGas);
                        })
                    }
                },
                SteamOutputTag = "PP_BLR8_MMBTU/HR",
                OuputToEnergyContentFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                {
                    //boiler 8 output is in MMBTUs per hour and it represents steam energy
                    Energy steamEnergy = Energy.FromMegabritishThermalUnits(dataPoint.Value);
                    return steamEnergy;
                })
            });

            boilerMappers.Add(new BoilerMapper
            {
                BoilerName = "Boiler 7",
                BoilerInputMappers = new List<BoilerInputMapper>()
                {
                    new BoilerInputMapper
                    {
                        InputName = "Boiler 7 Natural Gas Input",
                        InputTag = "PP_FI0721X",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { 
                            //boiler 7 input is in (standard) cubic kilo feet per hour and it represents a volume of natural gas
                            Volume naturalGas = Volume.FromKilocubicFeet(dataPoint.Value);
                            return NaturalGas.ToCo2Emissions(naturalGas);
                        })
                    }
                },
                SteamOutputTag = "PP_BLR7_MMBTU/HR",
                OuputToEnergyContentFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                {
                    //boiler 7 output is in MMBTUs per hour and it represents steam energy
                    Energy steamEnergy = Energy.FromMegabritishThermalUnits(dataPoint.Value);
                    return steamEnergy;
                })
            });

            boilerMappers.Add(new BoilerMapper
            {
                BoilerName = "Hospital Boiler",
                BoilerInputMappers = new List<BoilerInputMapper>()
                {
                    new BoilerInputMapper
                    {
                        InputName = "Hospital Boiler Natural Gas Input",
                        InputTag = "HBLR_GAS_FLOW",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { 
                            //Hospital boiler input is in (standard) cubic kilo feet per hour and it represents a volume of natural gas
                            Volume naturalGas = Volume.FromKilocubicFeet(dataPoint.Value);
                            return NaturalGas.ToCo2Emissions(naturalGas);
                        })
                    }
                },
                SteamOutputTag = "MC_195_MMBTU/HR_Calc",
                OuputToEnergyContentFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                {
                    //Hospital boiler output is in MMBTUs per hour and it represents steam energy
                    Energy steamEnergy = Energy.FromMegabritishThermalUnits(dataPoint.Value);
                    return steamEnergy;
                })
            });

            boilerMappers.Add(new BoilerMapper
            {
                BoilerName = "West Campus Boiler",
                BoilerInputMappers = new List<BoilerInputMapper>()
                {
                    new BoilerInputMapper
                    {
                        InputName = "West Campus Boiler Natural Gas Input",
                        InputTag = "PP_TB1_2_TB1_GAS_FLOW",
                        DataPointToCo2EmissionsFunction = new Func<InterpolatedDataPoint, Mass>((dataPoint) => { 
                            //West campus boiler input is in (standard) cubic feet per hour and it represents a volume of natural gas
                            Volume naturalGas = Volume.FromCubicFeet(dataPoint.Value);
                            return NaturalGas.ToCo2Emissions(naturalGas);
                        })
                    }
                },
                SteamOutputTag = "PP_TB1_2_STM_FLOW1",
                OuputToEnergyContentFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                {
                    //West campus boiler output is in kilo pounds per hour and it represents a mass of steam
                    Mass steam = Mass.FromKilopounds(dataPoint.Value);
                    return Steam.ToEnergyContent(steam, Steam.BTUS_PER_POUND_OF_STEAM_FROM_WEST_CAMPUS_BOILER);
                })
            });

            mapper.BoilerMappers = boilerMappers;

            return mapper;
        }


        private static CogeneratedElectricMapper BuildCogeneratedElectricMapper()
        {
            return new CogeneratedElectricMapper
            {
                TurbineProcessMappers = new List<SteamToElectricTurbineMapper>()
                {
                    new SteamToElectricTurbineMapper
                    {
                        TurbineName = "Turbine 6",
                        InvolvedDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet { Tag = "PP_tg6_throttle_mmbtu/hr" },
                            new TaggedDataSet { Tag = "PP_tg6_extraction_mmbtu/h" },
                            new TaggedDataSet { Tag = "pp_tg6_condensate_mmbtu/h" }
                        },
                        TurbineProcessDataToEnergyFunction = new Func<IEnumerable<TaggedDataPoint>, Energy>((datapoints) =>
                        {
                            InterpolatedDataPoint throttle = datapoints.Where(x => x.Tag == "PP_tg6_throttle_mmbtu/hr").First().DataPoint;
                            InterpolatedDataPoint extraction = datapoints.Where(x => x.Tag == "PP_tg6_extraction_mmbtu/h").First().DataPoint;
                            InterpolatedDataPoint condensate = datapoints.Where(x => x.Tag == "pp_tg6_condensate_mmbtu/h").First().DataPoint;

                            Energy steamEnergyUsedToProduceTheElectric = Energy.FromMegabritishThermalUnits(throttle.Value) -  Energy.FromMegabritishThermalUnits(extraction.Value) -  Energy.FromMegabritishThermalUnits(condensate.Value);
                            return steamEnergyUsedToProduceTheElectric;
                        })
                    },
                    new SteamToElectricTurbineMapper
                    {
                        TurbineName = "Turbine 7",
                        InvolvedDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet { Tag = "PP_TG7-7A-Throttle_MMBTU/HR" },
                            new TaggedDataSet { Tag = "PP_TG7-7A-Exhaust_MMBTU/HR" },
                            new TaggedDataSet { Tag = "PP_TG7-7B-Throttle_MMBTU/HR" },
                            new TaggedDataSet { Tag = "PP_TG7-7B-Exhaust_MMBTU/HR" }
                        },
                        TurbineProcessDataToEnergyFunction = new Func<IEnumerable<TaggedDataPoint>, Energy>((datapoints) =>
                        {
                            InterpolatedDataPoint throttleA = datapoints.Where(x => x.Tag == "PP_TG7-7A-Throttle_MMBTU/HR").First().DataPoint;
                            InterpolatedDataPoint exhaustA = datapoints.Where(x => x.Tag == "PP_TG7-7A-Exhaust_MMBTU/HR").First().DataPoint;
                            InterpolatedDataPoint throttleB = datapoints.Where(x => x.Tag == "PP_TG7-7B-Throttle_MMBTU/HR").First().DataPoint;
                            InterpolatedDataPoint exhaustB = datapoints.Where(x => x.Tag == "PP_TG7-7B-Exhaust_MMBTU/HR").First().DataPoint;

                            Energy steamEnergyUsedToProduceTheElectricA = Energy.FromMegabritishThermalUnits(throttleA.Value) -  Energy.FromMegabritishThermalUnits(exhaustA.Value);
                            Energy steamEnergyUsedToProduceTheElectricB = Energy.FromMegabritishThermalUnits(throttleB.Value) -  Energy.FromMegabritishThermalUnits(exhaustB.Value);
                            return steamEnergyUsedToProduceTheElectricA + steamEnergyUsedToProduceTheElectricB;
                        })
                    }
                    ,
                    new SteamToElectricTurbineMapper
                    {
                        TurbineName = "Turbine 8",
                        InvolvedDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet { Tag = "PP_TG8-8A-Throttle_MMBTU/HR" },
                            new TaggedDataSet { Tag = "PP_TG8-8A-Extracation_MMBTU/HR" },
                            new TaggedDataSet { Tag = "PP_TG8-8B-Throttle_MMBTU/HR" },
                            new TaggedDataSet { Tag = "PP_TG8-FT-8391" }
                        },
                        TurbineProcessDataToEnergyFunction = new Func<IEnumerable<TaggedDataPoint>, Energy>((datapoints) =>
                        {
                            InterpolatedDataPoint throttleA = datapoints.Where(x => x.Tag == "PP_TG8-8A-Throttle_MMBTU/HR").First().DataPoint;
                            InterpolatedDataPoint extractionA = datapoints.Where(x => x.Tag == "PP_TG8-8A-Extracation_MMBTU/HR").First().DataPoint;
                            InterpolatedDataPoint throttleB = datapoints.Where(x => x.Tag == "PP_TG8-8B-Throttle_MMBTU/HR").First().DataPoint;
                            InterpolatedDataPoint condensateB = datapoints.Where(x => x.Tag == "PP_TG8-FT-8391").First().DataPoint;

                            Energy steamEnergyUsedToProduceTheElectricA = Energy.FromMegabritishThermalUnits(throttleA.Value) -  Energy.FromMegabritishThermalUnits(extractionA.Value);
                            Energy steamEnergyUsedToProduceTheElectricB = Energy.FromMegabritishThermalUnits(throttleB.Value) -  Energy.FromMegabritishThermalUnits((condensateB.Value * 8.288 * 68) / 1000000);
                            return steamEnergyUsedToProduceTheElectricA + steamEnergyUsedToProduceTheElectricB;
                        })
                    }
                }
            };
        }


        private static SteamOverheadMapper BuildSteamOverheadMapper()
        {
            return new SteamOverheadMapper
            {
                FactorMappers = new List<SteamOverheadFactorMapper>()
                {
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_DA_Heater",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1203;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_FT0005",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1299;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_FT0001",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1299;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_FT0002",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1299;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP__11_ANNEX",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1299;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_BFPSTM",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 96;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_BLR7_8_HTR_FLO",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1384;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_BLR12_FT_007",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1384;
                            Mass steamMass = Mass.FromPounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_B10FWHDF",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1299;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    },
                    new SteamOverheadFactorMapper
                    {
                        SteamMeterTag = "PP_B11FWHDF",
                        DataPointToSteamEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
                        {
                            double btusPerPoundOfSteam = 1299;
                            Mass steamMass = Mass.FromKilopounds(dataPoint.Value);
                            return Energy.FromBritishThermalUnits(steamMass.Pounds * btusPerPoundOfSteam);
                        })
                    }
                }
            };
        }

        private static ElectricOverheadMapper BuildElectricOverheadMapper()
        {
            Func<InterpolatedDataPoint, Energy> dataPointToEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) =>
            {//All the electric meter tags express their data in KW, so this function can be shared.
                Energy electricEnergy = Energy.FromKilowattHours(dataPoint.Value); //Again technically the data is in KW not KWH. Essentially we are assuming the instaneous power remains constant for an hour
                return electricEnergy;                                             //There may be a tag for kwh which would be more accurate to use (because it is in kwh) but it's not a big deal right now.
            });

            return new ElectricOverheadMapper
            {
                FactorMappers = new List<ElectricOverheadFactorMapper>()
                {
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P163.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P164.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P165.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P166.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P167.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P168.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P169.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P171.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_PP_P500.KV2.KW_DMD", DataPointToEnergyFunction = dataPointToEnergyFunction }, //power plant aux meter
                    new ElectricOverheadFactorMapper { ElectricMeterTag = "EL_WCP_S266.KV2.KW", DataPointToEnergyFunction = dataPointToEnergyFunction },    //west campus boiler aux meter
                    new ElectricOverheadFactorMapper
                    {
                        ElectricMeterTag = null, IsConstantValue = true,
                        DataPointToEnergyFunction = new Func<InterpolatedDataPoint, Energy>((foo) => { return Energy.FromKilowattHours(-55); }) //Constant 55 kw deduct needed (not sure of exact reason why). 55 is an average derived from a monthly report.
                    },
                    new ElectricOverheadFactorMapper
                    {
                        ElectricMeterTag = "EL_PP_P502.KV2.KW_DMD",
                        DataPointToEnergyFunction = new Func<InterpolatedDataPoint, Energy>((dataPoint) => {
                            Energy electricEnergy = Energy.FromKilowattHours(dataPoint.Value * -1); //This is a deduct for the west campus boiler so multiple by -1 to make it negative (not sure of exact reason for the deduct)
                            return Energy.FromKilowattHours(-55);
                        })
                    },
                }
            };
        }

        private static ChilledWaterProductionMapper BuildChilledWaterProductionMapper()
        {
            return new ChilledWaterProductionMapper
            {
                Name = "UIowa Chilled Water",
                PlantMappers = new List<ChilledWaterPlantMapper>()
                {
                    new ChilledWaterPlantMapper
                    {
                        Name = "West Campus Chilled Water Plant",
                        ChilledWaterPlantInputDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet { Tag = "WCP_Electric" },
                            new TaggedDataSet { Tag = "WCP_07_FT_022" },
                            new TaggedDataSet { Tag = "WCP_08_FT_022" },
                            new TaggedDataSet { Tag = "WCP_09_FT_022" },
                        },
                        ChilledWaterOutputDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet
                            {
                                Tag = "WCP_01_FT_100",
                                FilterExpression = "'WCP_01_NS_100'=\"On\"",
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_02_FT_100",
                                FilterExpression = "'WCP_02_NS_100'=\"On\"",
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_YK_3A_Evap_Flow",
                                FilterExpression = "'WCP_03_NS_100'=\"TRUE\"",
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_YK_3B_Evap_Flow",
                                FilterExpression = "'WCP_04_NS_100'=\"TRUE\"",
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_CTV1_FT_100_ENG",
                                FilterExpression = "'WCP_CTV1_XS_100'=\"TRUE\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_CTV2_FT_100_ENG",
                                FilterExpression = "'WCP_CTV2_XS_100'=\"TRUE\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_05_FT_100_ENG",
                                FilterExpression = "'WCP_05_NS_100'=\"TRUE\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_06_FT_100_ENG",
                                FilterExpression = "'WCP_06_NS_100'=\"TRUE\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_00_FT_202",
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_07_FT_100",
                                FilterExpression = "'WCP_07_NS_100'=\"On\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_08_FT_100",
                                FilterExpression = "'WCP_08_NS_100'=\"On\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "WCP_09_FT_100",
                                FilterExpression = "'WCP_09_NS_100'=\"On\""
                            }
                        }
                    },
                    new ChilledWaterPlantMapper
                    {
                        Name = "Northwest Campus Chilled Water Plant",
                        ChilledWaterPlantInputDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet { Tag = "NWCP_Electric" },
                            new TaggedDataSet { Tag = "NWCP_CH1_PLC_AI_SteamSupplyFlow" }
                        },
                        ChilledWaterOutputDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet
                            {
                                Tag = "NWCP_CHNW_AI_EVAPWATERFLOW",
                                FilterExpression = "'NWCP_01_NS_100'=\"On\""
                            }
                        }
                    },
                    new ChilledWaterPlantMapper
                    {
                        Name = "North Campus Chilled Water Plant",
                        ChilledWaterPlantInputDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet { Tag = "NCP_Electric" }
                        },
                        ChilledWaterOutputDataSets = new List<TaggedDataSet>()
                        {
                            new TaggedDataSet
                            {
                                Tag = "NCP_03_FT_100",
                                FilterExpression = "'NCP_03_NS_160'=\"On\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "NCP_01_FT_100",
                                FilterExpression = "'NCP_01_NS_160'=\"On\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "NCP_02_FT_100",
                                FilterExpression = "'NCP_02_NS_160'=\"On\""
                            },
                            new TaggedDataSet
                            {
                                Tag = "NCP_04_FT_101",
                                FilterExpression = "'NCP_04_NS_160'=\"On\""
                            }
                            //new TaggedDataSet { Tag = "NCP_00_UI_202"  } //TODO: Free cooling normally not used at this plant. Still need gpm tag
                        }
                    }
                }
            };
        }
    }
}
