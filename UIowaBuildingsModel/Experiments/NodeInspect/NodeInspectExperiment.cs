using EmissionsMonitorModel.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmissionsMonitorModel.Experiments.NodeInspect
{
    public class NodeInspectExperiment
    {
        public int ExperimentId { get; set; }

        public int ModelId { get; set; }

        public int NodeId { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public string DataResolution { get; set; }

        public List<NodeInspectExperimentRecord> ExperimentRecords { get; set; }

        [NotMapped]
        public OverflowHandleStrategies? OverflowStrategy { get; set; }
    }
}
