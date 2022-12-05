using EmissionsMonitorModel.ConversionMethods;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UnifiedDataExplorer.ViewModel.ProcessModeling;

namespace UnifiedDataExplorer.Converters
{
    public class DataFunctionTypeToIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DataFunctionViewModel vm = (DataFunctionViewModel)value;
            Type type = vm?.GetBackingModel()?.GetType();
            switch (type?.Name)
            {
                case nameof(SteamEnergyFunction):
                    return "/Resources/Images/Steam.png";
                case nameof(ElectricEnergyFunction):
                    return "/Resources/Images/Lightening-Bolt.png";
                case nameof(ChilledWaterVolumeFunction):
                    return "/Resources/Images/Water.png";
                case nameof(Co2MassFunction):
                    return "/Resources/Images/CO2.png";
                case nameof(MoneyFunction):
                    return "/Resources/Images/Money.png";
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
