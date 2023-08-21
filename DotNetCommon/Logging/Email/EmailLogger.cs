using DotNetCommon.Extensions;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net.Mime;

namespace DotNetCommon.Logging.Email
{
    public class EmailLogger : ILogger
    {
        private string _mailHost => _configuration.MailHost;

        private readonly ClientInfo _clientInfo;
        private readonly EmailLoggerConfig _configuration;

        public EmailLogger(ClientInfo clientInfo, EmailLoggerConfig configuration)
        {
            if (clientInfo == null) throw new ArgumentNullException(nameof(clientInfo));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            _clientInfo = clientInfo;
            _configuration = configuration;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _configuration.CanLog(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (_configuration.CanLog(logLevel))
            {
                IDictionary<string, object> stateAsDictionary = (state as IEnumerable<KeyValuePair<string, object>>)?.ToDictionary(i => i.Key, i => i.Value);
                string message;
                if (stateAsDictionary != null && stateAsDictionary.ContainsKey("{OriginalFormat}")) message = stateAsDictionary["{OriginalFormat}"].ToString();
                else message = String.Empty;

                using (MailMessage mailMessage = BuildMailMessage(logLevel, eventId, message, exception))
                {
                    using (SmtpClient smtp = new SmtpClient(_mailHost))
                    {
                        smtp.Send(mailMessage);
                    }
                }
            }
        }

        private MailMessage BuildMailMessage(LogLevel logLevel, EventId eventId, string message, Exception exception)
        {
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_configuration.MessageFrom);
            string[] addresses = _configuration.MessageTo.Split(';');
            foreach (string address in addresses)
            {
                if (!string.IsNullOrWhiteSpace(address)) mailMessage.To.Add(new MailAddress(address));
            }
            mailMessage.Subject = _configuration.Subject;
            mailMessage.Priority = MailPriority.Normal;

            string color = this.ConvertCategoryToColor(logLevel);

            string htmlView = $@"<h3 style='color:{color};'><b>Software: {_clientInfo.Software}<br/>Log Category: {logLevel}</b></h3>";
            if (eventId != null && !String.IsNullOrWhiteSpace(eventId.Name)) htmlView = htmlView + $@"<p><b>Event:</b> <br /> {eventId.Name}</p>";
            if (!String.IsNullOrWhiteSpace(message)) htmlView = htmlView + $@"<p><b>Log Message:</b> <br /> {message}</p>";
            htmlView = htmlView + $@"<p><b>Client Info:</b> <br /> User = {_clientInfo.User} Version = {_clientInfo.Version} Machine = {_clientInfo.Machine}</p>";

            if (exception != null) htmlView = htmlView + exception.ToHtmlString();

            mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(htmlView, null, MediaTypeNames.Text.Html));
            mailMessage.IsBodyHtml = false;
            return mailMessage;
        }

        private string ConvertCategoryToColor(LogLevel logLevel)
        {
            string color = "";
            switch (logLevel)
            {
                case LogLevel.Error:
                case LogLevel.Critical:
                    color = "red";
                    break;
                case LogLevel.Warning:
                    color = "orange";
                    break;
                case LogLevel.Information:
                    color = "blue";
                    break;
                case LogLevel.Debug:
                default:
                    color = "black";
                    break;
            }

            return color;
        }
    }
}
