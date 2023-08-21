using DotNetCommon.Bases;
using DotNetCommon.Extensions;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization;

namespace DotNetCommon.Logging
{
    public class LoggerConfiguration : ValidatingObject
    {
        public LoggerConfiguration()
        {
            LogLevel = new LogLevelConfig { Default = Microsoft.Extensions.Logging.LogLevel.Information.ToDescription() };
        }

        public LogLevelConfig LogLevel { get; set; }

        public LogLevel DefaultLogLevel => Enum.Parse<LogLevel>(LogLevel.Default);

        internal bool CanLog(LogLevel logCategory)
        {
            return logCategory >= DefaultLogLevel;
        }
    }
}
