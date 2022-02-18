using Microsoft.Extensions.Configuration;
using System.Reflection;
using DotNetCommon.Extensions;
using System.Text.Json;

namespace UIowaBuildingsModel
{
    public class Config
    {
        private readonly IConfiguration _rawConfig;

        public Config()
        {
        }

        public Config(IConfiguration rawConfig)
        {
            _rawConfig = rawConfig;

            //Autobinding appsettings data to the strongly-typed properties of this object
            IEnumerable<PropertyInfo> props = typeof(Config).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string rawValue = _rawConfig[prop.Name];
                prop.SetValueWithTypeRespect(this, rawValue);
            }

            string json = File.ReadAllText(this.CredentialsFile);
            Config temp = JsonSerializer.Deserialize<Config>(json);
            this.UserName = temp.UserName;
            this.Password = temp.Password;            
        }

        public string CredentialsFile { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Base64AuthString => "Basic " + "SU9XQVxyanBvbGljaGVyaTpUdXJCdTEmblRoOURyMCE=";

        public string PiWebApiBase { get; set; }
    }
}