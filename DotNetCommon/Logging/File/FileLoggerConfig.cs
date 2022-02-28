using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace DotNetCommon.Logging.File
{
    public class FileLoggerConfig
    {

        public FileLoggerConfig(string logFileDirectory, string logFileName)
        {
            LogFileDirectory = logFileDirectory.TrimEnd(Path.DirectorySeparatorChar);
            LogFileName = logFileName;
        }

        public string LogFileDirectory { get; }
        public string LogFileName { get; }
        public string LogFileFullPath => LogFileDirectory + Path.DirectorySeparatorChar + LogFileName;

        public bool CanLog(LogLevel logLevel)
        {
            return true;
        }
    }
}
