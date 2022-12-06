using Microsoft.Extensions.Configuration;
using System.Reflection;
using DotNetCommon.Extensions;

namespace UIowaBuildingsModelConsoleApp
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
        }

        public string DefaultConnection { get; set; }

        public string HourlyEmissionsReportRootAssetLink { get; set; }
    }
}