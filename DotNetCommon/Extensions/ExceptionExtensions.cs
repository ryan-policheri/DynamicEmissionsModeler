namespace DotNetCommon.Extensions
{
    public static class ExceptionExtensions
    {
        public static string ToHtmlString(this Exception exception)
        {
            if (exception == null) return null;
            else
            {
                string htmlString = $@"<p><b>Exception Message:</b> <br /> {exception.Message}</p>" +
                $@"<p><b>Stack Trace:</b> <br /> {exception.StackTrace}</p>";

                int counter = 1;
                while (exception.InnerException != null)
                {
                    htmlString = htmlString + $@"<p><b>Inner Exception {counter++} Message:</b> <br /> {exception.InnerException?.Message}</p>";
                    exception = exception.InnerException;
                }
                return htmlString;
            }
        }
    }
}
