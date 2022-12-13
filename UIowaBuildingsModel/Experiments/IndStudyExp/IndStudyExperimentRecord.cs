namespace EmissionsMonitorModel.Experiments.IndStudyExp
{
    public class IndStudyExperimentRecord
    {
        public IndStudyExperiment Experiment { get; set; }

        public int ExperimentId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public double TotalEmissionsInMetricTons { get; set; }

        public double TotalCostInDollars { get; set; }

        public double SteamTotalInMMBTU { get; set; }

        public double SteamEmissionsInMetricTons { get; set; }

        public double SteamCostInDollars { get; set; }

        public double SteamEmissionsFactorInKilogramsCo2PerMMBTU { get; set; }

        public double SteamCostDollarsPerMMBTU { get; set; }

        public double ElectricTotalInKwh { get; set; }

        public double ElectricEmissionsInMetricTons { get; set; }

        public double ElectricCostInDollars { get; set; }

        public double ElectricEmissionsFactorInKilogramsCo2PerKwh { get; set; }

        public double ElectricCostPerKwh { get; set; }

        public double ChilledWaterTotalInGallons { get; set; }

        public double ChilledWaterEmissionsInMetricTons { get; set; }

        public double ChilledWaterCostInDollars { get; set; }

        public double ChilledWaterEmissionsFactorInKilogramsCo2PerGallon { get; set; }

        public double ChilledWaterCostPerGallon { get; set; }
    }
}
