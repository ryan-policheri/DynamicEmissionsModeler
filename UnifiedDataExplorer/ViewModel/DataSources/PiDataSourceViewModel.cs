using DotNetCommon.MVVM;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class PiDataSourceViewModel : ViewModelBase
    {
        public string PiWebApiUrl { get; set; }

        public string PiUserName { get; set; }

        public string PiPassword { get; set; }
    }
}