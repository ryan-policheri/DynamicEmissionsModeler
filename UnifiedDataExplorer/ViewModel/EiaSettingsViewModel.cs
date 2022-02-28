using DotNetCommon.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedDataExplorer.ViewModel
{
    public class EiaSettingsViewModel : ViewModelBase
    {
        private string _eiaBaseUrl;
        public string EiaBaseUrl
        {
            get { return _eiaBaseUrl; }
            set 
            {
                _eiaBaseUrl = value;
                OnPropertyChanged();
            }
        }


        private string _eiaApiKey;
        public string EiaApiKey
        {
            get { return _eiaApiKey; }
            set
            {
                _eiaApiKey = value;
                OnPropertyChanged();
            }
        }
    }
}
