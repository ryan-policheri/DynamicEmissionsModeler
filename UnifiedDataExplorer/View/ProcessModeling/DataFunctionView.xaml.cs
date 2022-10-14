using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnifiedDataExplorer.ViewModel.ProcessModeling;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.View.ProcessModeling
{
    /// <summary>
    /// Interaction logic for DataFunctionView.xaml
    /// </summary>
    public partial class DataFunctionView : UserControl
    {
        public DataFunctionViewModel ViewModel => this.DataContext as DataFunctionViewModel;

        public DataFunctionView()
        {
            InitializeComponent();
        }

        private void OnDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && ViewModel != null)
            {
                FunctionFactorViewModel senderVm = (sender as TextBlock)?.DataContext as FunctionFactorViewModel;
                ViewModel.OpenFactor.Execute(senderVm);
            }
        }
    }
}
