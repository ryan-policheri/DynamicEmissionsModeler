using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorModel.Exceptions
{
    public class NodeOverflowException : InvalidOperationException
    {
        public NodeOverflowException(NodeOverflowError error, string? message, Exception? innerException) : base(message, innerException)
        {
            Error = error;
        }

        public NodeOverflowError Error { get; }
    }

    public enum OverflowHandleStrategies
    {
        ExcludeTimeslot = 0,
        UsePrevious = 1
    }
}
