using DotNetCommon.Extensions;
using PiModel;
using UnitsNet;

namespace EmissionsMonitorModel
{
    public class ElectricOverheadMapper
    {
        public IEnumerable<ElectricOverheadFactorMapper> FactorMappers { get; set; }

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            IEnumerable<DateTimeOffset> outputDataHours = this.FactorMappers.Where(x => !x.IsConstantValue).First().DataPoints.Select(x => x.Timestamp);

            foreach(ElectricOverheadFactorMapper factorMapper in this.FactorMappers)
            {
                if(!factorMapper.IsConstantValue)
                {
                    bool allHoursMatch = outputDataHours.AllHoursMatch(factorMapper.DataPoints.Select(x => x.Timestamp));
                    if (!allHoursMatch) throw new InvalidDataException("The timestamps between the associated data sets do not match");
                }
            }

            return outputDataHours.ToList().Copy();
        }

        public IEnumerable<ElectricOverhead> PackageDataIntoElectricOverheads()
        {
            ICollection<ElectricOverhead> result = new List<ElectricOverhead>();

            foreach (DateTimeOffset hour in this.EnumerateDataHours())
            {
                ElectricOverhead electricOverhead = new ElectricOverhead();
                Energy hourlyElectricOverhead = Energy.Zero;

                foreach (ElectricOverheadFactorMapper factorMapper in this.FactorMappers)
                {
                    if(!factorMapper.IsConstantValue)
                    {
                        InterpolatedDataPoint dataPoint = factorMapper.DataPoints.FindMatchingHour(hour);
                        Energy electricEnergy = factorMapper.DataPointToEnergyFunction(dataPoint);
                        hourlyElectricOverhead += electricEnergy;
                    }
                    else
                    {
                        Energy electricEnergy = factorMapper.DataPointToEnergyFunction(null);
                        hourlyElectricOverhead += electricEnergy;
                    }
                }

                electricOverhead.Timestamp = hour;
                electricOverhead.ElectricEnergy = hourlyElectricOverhead;
                result.Add(electricOverhead);
            }

            return result;
        }
    }
}