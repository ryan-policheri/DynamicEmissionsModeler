using System.Windows;
using System.Windows.Controls;
using UIowaBuildingsModel;
using UnifiedDataExplorer.ViewModel;

namespace UnifiedDataExplorer.View
{
    public partial class HourlyEmissionsReportParametersView : UserControl
    {
        private HourlyEmissionsReportParametersViewModel ViewModel => ((HourlyEmissionsReportParametersViewModel)(this.DataContext));

        public HourlyEmissionsReportParametersView()
        {
            InitializeComponent();
        }

        private void Flipper_IsFlippedChanged(object sender, RoutedPropertyChangedEventArgs<bool> args)
        {
            ElectricGridStrategy strategy = ElectricGridStrategy.MisoHourly;
            if (args.NewValue == true) strategy = ElectricGridStrategy.MidAmericanAverageFuelMix;
            ViewModel.GridStrategy = strategy;            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.DoExecute = true;
            Window modalWindow = Window.GetWindow(this);
            modalWindow.Close();
        }
    }
}
