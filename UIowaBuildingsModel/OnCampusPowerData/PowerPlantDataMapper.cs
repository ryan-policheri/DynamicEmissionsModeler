using DotNetCommon.Extensions;

namespace UIowaBuildingsModel
{
    public class PowerPlantDataMapper //NOTE there will also be data that is not physically in the power plant (I.E. boilers or solar panels elsewhere on campus that are connected to the system.)
    {                                 //Perhaps it would make sense to parse out distrubuted generation/production from the central plant and encapsulate all in a class called like PowerInfrastructureMapper
        public string Name { get; set; }

        public SteamProductionMapper SteamProductionMapper { get; set; } //Data of fuel inputs and steam output to each boiler in the powerlant

        public CogeneratedElectricMapper CogeneratedElectricMapper { get; set; } //Data about how much steam went to produce cogenerated electric

        public SteamOverheadMapper SteamOverheadMapper { get; set; } //Data about how much steam the power plant used internally to operate

        public ElectricOverheadMapper ElectricOverheadMapper { get; set; } //Data about how much electric the power plant used internally to operate

        public ChilledWaterProductionMapper ChilledWaterMapper { get; set; } //Data about how much went steam and electricity went into producing chilled water and how much chilled water came out

        public IEnumerable<DateTimeOffset> EnumerateDataHours()
        {
            IEnumerable<DateTimeOffset> dataHours = SteamProductionMapper.EnumerateDataHours();
            bool allHoursMatch = dataHours.AllHoursMatch(CogeneratedElectricMapper.EnumerateDataHours());
            if (allHoursMatch) allHoursMatch = dataHours.AllHoursMatch(SteamOverheadMapper.EnumerateDataHours());
            if (allHoursMatch) allHoursMatch = dataHours.AllHoursMatch(ElectricOverheadMapper.EnumerateDataHours());
            
            if(!allHoursMatch) throw new InvalidDataException("The timestamps between the associated data sets do not match");
            else return dataHours.ToList().Copy();
        }
    }
}
