using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionsMonitorModel.Experiments.DailyCarbonTrend
{
    public class DailyCarbonExperiment
    {
        public int ExperimentId { get; set; }

        public int ModelId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        [NotMapped]
        public IEnumerable<int>? NodeIds { get; set; }

        public List<DailyCarbonExperimentRecord> ExperimentRecords { get; set; }
    }
}
