using DotNetCommon.Extensions;

namespace UIowaBuildingsModel
{
    public class SteamProductionMapper
    {
        public string Name { get; set; }

        public IEnumerable<BoilerMapper> BoilerMappers { get; set; } //Data of fuel inputs and steam output to each boiler on campus

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            IEnumerable<DateTimeOffset> dataHours = this.BoilerMappers.First().EnumerateDataHours();

            foreach (BoilerMapper boilerMapper in this.BoilerMappers)
            {
                bool allHoursMatch = dataHours.AllHoursMatch(boilerMapper.EnumerateDataHours());
                if(!allHoursMatch) throw new InvalidDataException("The timestamps between the associated data sets do not match");
            }

            return dataHours.ToList().Copy();
        }
    }
}