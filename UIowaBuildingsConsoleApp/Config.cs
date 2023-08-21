using Microsoft.Extensions.Configuration;
using System.Reflection;
using DotNetCommon.Extensions;

namespace UIowaBuildingsModelConsoleApp
{
    public class Config
    {
        public readonly IConfiguration RawConfig;

        public Config()
        {
        }

        public Config(IConfiguration rawConfig)
        {
            RawConfig = rawConfig;

            //Autobinding appsettings data to the strongly-typed properties of this object
            IEnumerable<PropertyInfo> props = typeof(Config).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string rawValue = RawConfig[prop.Name];
                prop.SetValueWithTypeRespect(this, rawValue);
            }
        }

        public string DefaultConnection { get; set; }

        public string HourlyEmissionsReportRootAssetLink { get; set; }
    }
}