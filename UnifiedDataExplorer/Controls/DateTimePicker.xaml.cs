using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace UnifiedDataExplorer.Controls
{//https://gist.github.com/Apflkuacha/406e755c8b42a70b7ab138e6b985bcdf
    /// Changes and improvements made =>
    /// 1. Removed AM/PM chooser and made time selectable from 6:00 to 18:00
    /// 2. Popup improvements
    /// 3. Added SelectedDate DependencyProperty
    ///
    /// Things that will be needed for this control to work properly (and look good :) ) =>
    /// 1. A bitmap image 32x32 added as an embedded resource
    ///
    /// Licensing =>
    /// The Code Project Open License (CPOL)
    /// http://www.codeproject.com/info/cpol10.aspx

    public partial class DateTimePicker : UserControl
    {
        private const string DateTimeFormat = "dd.MM.yyyy HH:mm";

        #region "Properties"

        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        #endregion

        #region "DependencyProperties"

        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate",
            typeof(DateTime), typeof(DateTimePicker), new FrameworkPropertyMetadata(DateTime.Now, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        public DateTimePicker()
        {
            InitializeComponent();
            CalDisplay.SelectedDatesChanged += CalDisplay_SelectedDatesChanged;
            CalDisplay.SelectedDate = DateTime.Now.AddDays(1);

            BitmapSource ConvertGDI_To_WPF(Bitmap bm)
            {
                BitmapSource bms = null;
                IntPtr h_bm = IntPtr.Zero;
                h_bm = bm.GetHbitmap();
                bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                bms.Freeze();
                h_bm = IntPtr.Zero;
                return bms;
            }
           // Bitmap bitmap1 = Properties.Resources.DateTimePicker;
            //bitmap1.MakeTransparent(Color.Black);
            //CalIco.Source = ConvertGDI_To_WPF(bitmap1);
        }

        #region "EventHandlers"

        private void CalDisplay_SelectedDatesChanged(object sender, EventArgs e)
        {
            var hours = (Hours?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
            var minutes = (Min?.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "0";
            TimeSpan timeSpan = TimeSpan.Parse(hours + ":" + minutes);
            if (CalDisplay.SelectedDate.Value.Date == DateTime.Today.Date && timeSpan.CompareTo(DateTime.Now.TimeOfDay) < 0)
            {
                timeSpan = TimeSpan.FromHours(DateTime.Now.Hour + 1);
            }
            var date = CalDisplay.SelectedDate.Value.Date + timeSpan;
            DateDisplay.Text = date.ToString(DateTimeFormat);
            SelectedDate = date;
        }

        private void SaveTime_Click(object sender, RoutedEventArgs e)
        {
            CalDisplay_SelectedDatesChanged(SaveTime, EventArgs.Empty);
            PopUpCalendarButton.IsChecked = false;
        }

        private void Time_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalDisplay_SelectedDatesChanged(sender, e);
        }

        private void CalDisplay_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {   // that it's not necessary to click twice after opening the calendar  https://stackoverflow.com/q/6024372
            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        #endregion
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }
    }
}