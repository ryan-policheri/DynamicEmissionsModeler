namespace DotNetCommon.SystemFunctions
{
    public class ProcessStats
    {
        public ProcessStats()
        {
        }

        public ProcessStats(DateTime startTime, DateTime endTime, bool didExit)
        {
            StartTime = startTime;
            EndTime = endTime;
            DidFinish = didExit;
        }

        public DateTime StartTime { get; }

        public DateTime EndTime { get; }

        public bool DidFinish { get; }

        public int SecondsEllapsed => (DidFinish ? (EndTime - StartTime).Seconds : -1);
        public double MillisecondsEllapsed => (DidFinish ? (EndTime - StartTime).TotalMilliseconds : -1);
    }
}
