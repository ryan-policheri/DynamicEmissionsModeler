using MaterialDesignThemes.Wpf;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace UnifiedDataExplorer.Controls
{
    public partial class MaterialDateTimePicker : UserControl
    {
        public MaterialDateTimePicker()
        {
            InitializeComponent();
        }

        public void CalendarDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            Calendar.SelectedDate = SelectedDateTime.Date;
            Clock2.Time = SelectedDateTime;
        }

        public void CalendarDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, "1")) return;

            if (!Calendar.SelectedDate.HasValue)
            {
                eventArgs.Cancel();
                return;
            }

            var dateSource = Calendar.SelectedDate.Value;
            var timeSource = Clock2.Time.TimeOfDay;
            SelectedDateTime = new DateTime(dateSource.Year, dateSource.Month, dateSource.Day, timeSource.Hours, timeSource.Minutes, timeSource.Seconds, DateTimeKind.Unspecified);
            DialogButton.Content = SelectedDateTime.ToString("g");
        }

        public DateTime SelectedDateTime
        {
            get { return (DateTime)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartStartDateTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime), typeof(MaterialDateTimePicker), new PropertyMetadata(DateTime.Today));

        private void MaterialDateTimePicker_OnLoaded(object sender, RoutedEventArgs e)
        {
            DialogButton.Content = SelectedDateTime.ToString("g");
        }
    }
}
