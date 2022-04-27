using DotNetCommon.Extensions;
using PiModel;
using System.Data;
using UnitsNet;

namespace UIowaBuildingsModel
{
    public class ChilledWaterProductionMapper
    {
        public string Name { get; set; }

        public IEnumerable<ChilledWaterPlantMapper> PlantMappers { get; set; }

        public IEnumerable<ChilledWaterProduced> PackageIntoChilledWaterProduced(DateTimeOffset startTime, DateTimeOffset endTime)
        {
            ICollection<ChilledWaterProduced> chilledWaterProduceds = new List<ChilledWaterProduced>();
            IEnumerable<DateTimeOffset> hours = startTime.EnumerateHoursUntil(endTime);

            foreach(DateTimeOffset hour in hours)
            {
                ChilledWaterProduced chilledWaterProduced = new ChilledWaterProduced();
                chilledWaterProduced.Timestamp = hour;

                Energy steamEnergyUsed = Energy.Zero;
                Energy electricEnergyUsed = Energy.Zero;
                Volume waterProduced = Volume.Zero;

                foreach (ChilledWaterPlantMapper mapper in PlantMappers)
                {
                    foreach (TaggedDataSet dataSet in mapper.ChilledWaterPlantInputDataSets)
                    {
                        InterpolatedDataPoint dataPoint = dataSet.DataPoints.FindMatchingHour(hour);
                        if (dataPoint != null)
                        {
                            if (dataSet.Units.CapsAndTrim() == "LBS/HR") //STEAM
                            {
                                steamEnergyUsed += Energy.FromBritishThermalUnits(dataPoint.Value * 1224); //1224 enthalpy value from George
                            }
                            else if (dataSet.Units.CapsAndTrim() == "KW") //ELECTRIC
                            {
                                electricEnergyUsed += Energy.FromKilowattHours(dataPoint.Value); //Assuming same load for an hour
                            }
                            else throw new NotImplementedException($"{dataSet.Units} not implemented in chilled water input packaging");
                        }
                    }

                    foreach (TaggedDataSet dataSet in mapper.ChilledWaterOutputDataSets)
                    {
                        InterpolatedDataPoint dataPoint = dataSet.DataPoints.TryFindMatchingHour(hour);
                        if (dataPoint != null)
                        {
                            if (dataSet.Units.CapsAndTrim() == "GPM")
                            {
                                waterProduced += Volume.FromUsGallons(dataPoint.Value); //TODO Is pi using US Gallons or imperial gallons??
                            }
                            else throw new NotImplementedException($"{dataSet.Units} not implemented in chilled water output packaging");
                        }
                    }
                }

                chilledWaterProduced.SteamEnergyUsed = steamEnergyUsed;
                chilledWaterProduced.ElectricEnergyUsed = electricEnergyUsed;
                chilledWaterProduced.ChilledWaterVolume = waterProduced;
                chilledWaterProduceds.Add(chilledWaterProduced);
            }

            return chilledWaterProduceds;
        }
    }
}
