using DotNetCommon.MVVM;

namespace UnifiedDataExplorer.ViewModel
{
    public class JsonDisplayViewModel : ViewModelBase
    {
        private string _header;
        public string Header
        {
            get { return _header; }
            set
            {
                _header = value;
                OnPropertyChanged();
            }
        }

        public string HeaderDetail { get; set; }

        public bool IsCloseable => true;

        public string Json { get; set; }
    }
}