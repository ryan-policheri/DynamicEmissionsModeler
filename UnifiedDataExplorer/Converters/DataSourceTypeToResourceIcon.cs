using System;
using System.Globalization;
using System.Windows.Data;
using EmissionsMonitorModel.DataSources;

namespace UnifiedDataExplorer.Converters
{
    public class DataSourceTypeToResourceIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataSourceType sourceType = (DataSourceType)value;
            switch (sourceType)
            {
                case DataSourceType.Eia:
                    return "/Resources/Images/EIA_Logo.png";
                case DataSourceType.Pi:
                    return "/Resources/Images/Pi_Logo.png";
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
