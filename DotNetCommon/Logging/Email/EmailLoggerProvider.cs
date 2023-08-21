using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCommon.Logging.Email
{
    public class EmailLoggerProvider : ILoggerProvider
    {
        private readonly ClientInfo _clientInfo;
        private readonly EmailLoggerConfig _config;

        private EmailLogger _logger;

        public EmailLoggerProvider(ClientInfo clientInfo, EmailLoggerConfig config)
        {
            _clientInfo = clientInfo;
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            _logger = new EmailLogger(_clientInfo, _config);
            return _logger;
        }

        public void Dispose()
        {
            _logger = null;
        }
    }
}
