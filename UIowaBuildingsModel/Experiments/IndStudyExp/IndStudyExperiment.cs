namespace EmissionsMonitorModel.Experiments.IndStudyExp
{
    public class IndStudyExperiment
    {
        public int ExperimentId { get; set; }

        public int ModelId { get; set; }

        public int FinalSteamNodeId { get; set; }

        public int FinalElectricNodeId { get; set; }

        public int FinalChilledWaterNodeId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public string DataResolution { get; set; }

        public List<IndStudyExperimentRecord> ExperimentRecords { get; set; }
    }
}
