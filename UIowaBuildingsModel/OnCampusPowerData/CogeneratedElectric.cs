﻿using UnitsNet;

namespace EmissionsMonitorModel
{
    public class CogeneratedElectric
    {
        public DateTimeOffset Timestamp { get; set; }
        public IEnumerable<string> SteamTurbineNames { get; set; }
        public Energy SteamEnergyUsed { get; set; }
    }
}