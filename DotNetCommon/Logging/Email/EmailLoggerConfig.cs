using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace DotNetCommon.Logging.Email
{
    public class EmailLoggerConfig : LoggerConfiguration
    {
        [Required]
        public string MailHost { get; set; }
        [Required]
        public string MessageFrom { get; set; }
        [Required]
        public string MessageTo { get; set; }
        [Required]
        public string Subject { get; set; }

    }
}
