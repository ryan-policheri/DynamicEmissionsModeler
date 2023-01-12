namespace EmissionsMonitorModel.Experiments.NodeInspect
{
    public class NodeInspectExperimentRecord
    {
        public NodeInspectExperiment Experiment { get; set; }

        public int ExperimentId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public double ProductTotalInDefaultUnit { get; set; }

        public string ProductDefaultUnit { get; set; }

        public double CO2EmissionsInKilograms { get; set; }

        public double FuelCostInDollars { get; set; }
        public double CO2EmissionsPerDefaultProductUnit { get; set; }
        public double FuelCostPerDefaultProductUnit { get; set; }
    }
}
