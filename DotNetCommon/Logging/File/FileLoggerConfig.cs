using System.ComponentModel.DataAnnotations;
using DotNetCommon.SystemHelpers;

namespace DotNetCommon.Logging.File
{
    public class FileLoggerConfig : LoggerConfiguration
    {
        [Required]
        public string? LogDirectory { get; set; }
        [Required]
        public string? LogFileName { get; set;  }
        public string LogFileFullPath => SystemFunctions.CombineDirectoryComponents(LogDirectory, LogFileName);
    }
}
