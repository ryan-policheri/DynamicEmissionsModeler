namespace DotNetCommon.Logging
{
    public class ClientInfo
    {
        public string Software { get; set; }
        public string Version { get; set; }
        public string User { get; set; }
        public string Machine { get; set; }
        public string Environment { get; set; }

        public ClientInfo(string software, string version, string user, string machine, string environment)
        {
            Software = software;
            Version = version;
            User = user;
            Machine = machine;
            Environment = environment;
        }
    }
}
