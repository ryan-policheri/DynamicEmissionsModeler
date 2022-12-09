namespace EmissionsMonitorModel.Experiments.DailyCarbonTrend
{
    public class DailyCarbonExperimentRecord
    {
        public DailyCarbonExperiment Experiment { get; set; }

        public int ExperimentId { get; set; }

        public DateTimeOffset Date { get; set; }

        public double Co2InKilograms { get; set; }

        public double Co2InMetricTons { get; set; }

        public double FuelCostInDollars { get; set; }
    }
}
