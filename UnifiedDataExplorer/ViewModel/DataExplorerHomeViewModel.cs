using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class DataExplorerHomeViewModel : RobustViewModelBase
    {
        public DataExplorerHomeViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            AddDataSource = new DelegateCommand(OnAddDataSource);
        }

        public string Header => "Home";
        public string HeaderDetail => "Open an explorer to find data!";
        public bool IsCloseable => false;

        public ICommand AddDataSource { get; }

        private void OnAddDataSource()
        {
            DataSourceManagerViewModel viewModel = this.Resolve<DataSourceManagerViewModel>();
            this.DialogService.ShowModalWindow<DataSourceManagerViewModel>(viewModel);
        }
    }
}
