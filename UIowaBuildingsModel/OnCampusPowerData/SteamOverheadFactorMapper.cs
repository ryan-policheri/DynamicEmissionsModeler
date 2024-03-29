﻿using UnitsNet;
using PiModel;

namespace EmissionsMonitorModel
{
    public class SteamOverheadFactorMapper
    {
        public string SteamMeterTag { get; set; }

        public Func<InterpolatedDataPoint, Energy> DataPointToSteamEnergyFunction { get; set; }

        public IEnumerable<InterpolatedDataPoint> DataPoints { get; set; }
    }
}
