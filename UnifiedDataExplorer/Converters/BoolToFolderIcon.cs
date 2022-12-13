using System;
using System.Globalization;
using System.Windows.Data;

namespace UnifiedDataExplorer.Converters
{
    public class BoolToFolderIcon : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isExpanded = (bool)value;
            if (isExpanded) return "FolderOpen";
            else return "Folder";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
