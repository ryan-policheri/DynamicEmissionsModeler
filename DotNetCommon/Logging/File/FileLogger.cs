using System.Text;
using Microsoft.Extensions.Logging;

namespace DotNetCommon.Logging.File
{
    public class FileLogger : ILogger
    {
        private readonly FileLoggerConfig _config;
        private readonly Queue<string> _logQueue;

        public FileLogger(FileLoggerConfig config)
        {
            _config = config;
            _logQueue = new Queue<string>();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_config.CanLog(logLevel))
            {
                IDictionary<string, object> stateAsDictionary = (state as IEnumerable<KeyValuePair<string, object>>)?.ToDictionary(i => i.Key, i => i.Value);
                string message;
                if (stateAsDictionary != null && stateAsDictionary.ContainsKey("{OriginalFormat}")) message = stateAsDictionary["{OriginalFormat}"].ToString();
                else message = String.Empty;

                if(!String.IsNullOrWhiteSpace(message))
                {
                    foreach(var item in stateAsDictionary)
                    {
                        message = message.Replace(item.Key, item.Value.ToString());
                    }
                }

                QueueEntry(logLevel, eventId, message, exception);
                if (EnsureDirectory())
                {
                    WriteQueue();
                }
            }
        }

        private void QueueEntry(LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            string line = BuildEntryText(logLevel, eventId, message, exception);
            _logQueue.Enqueue(line);
        }

        private string BuildEntryText(LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"Time: {DateTime.Now} Category: {logLevel.ToString()} Message: {message}");
            if (eventId != null && !String.IsNullOrWhiteSpace(eventId.Name)) builder.Append($" Event: {eventId.Name}");

            bool loggedMoreText = false;
            if (exception != null)
            {
                loggedMoreText = true;
                builder.AppendLine();
                builder.AppendLine($"Exception Message: {exception.Message}");
                builder.AppendLine($"StackTrace:{Environment.NewLine}{exception.StackTrace}");
                builder.Append($"Inner Exception Message: {exception.InnerException?.Message}");
            }

            if (loggedMoreText)
            {
                builder.AppendLine(); //Add an extra line for spacing
            }

            return builder.ToString();

        }

        private bool EnsureDirectory()
        {
            try
            {
                Directory.CreateDirectory(_config.LogFileDirectory);
                return true;
            }
            catch (IOException ioe)
            {
                return false;
            }
        }

        private void WriteQueue()
        {
            if (_logQueue.Count == 0) return;

            try
            {
                using (StreamWriter writer = new StreamWriter(_config.LogFileFullPath, true))
                {
                    while (_logQueue.Count > 0)
                    {
                        string line = _logQueue.Peek();
                        writer.WriteLine(line);
                        _logQueue.Dequeue();
                    }
                }
            }
            catch (IOException ioe)
            {
                return;
            }
        }
    }
}
