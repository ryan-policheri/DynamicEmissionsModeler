using Microsoft.Extensions.Logging;

namespace DotNetCommon.Logging.File
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly FileLoggerConfig _config;
        private FileLogger _logger;

        public FileLoggerProvider(FileLoggerConfig config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            _logger = new FileLogger(_config);
            return _logger;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
