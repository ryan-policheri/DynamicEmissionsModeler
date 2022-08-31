using System.Collections.Generic;
using System.Collections.ObjectModel;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class DataSourceManagerViewModel : RobustViewModelBase
    {
        private ICollection<DataSourceBase> _dataSources;

        public DataSourceManagerViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            _dataSources = new List<DataSourceBase>();
        }

        public ObservableCollection<DataSourceBase> DataSources { get; }
    }
}