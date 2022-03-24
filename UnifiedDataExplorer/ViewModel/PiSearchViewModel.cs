using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using PiServices;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiSearchViewModel : RobustViewModelBase
    {
        private PiHttpClient _client;

        public PiSearchViewModel(PiHttpClient client, RobustViewModelDependencies facade) : base(facade)
        {
            _client = client;
            PiPointSearchCommand = new DelegateCommand(OnPiPointSearch);
        }

        public string Header => "PI Search";
        public string HeaderDetail => "Search for PI tags";
        public bool IsCloseable => false;


        private string _piPointSearchText;
        public string PiPointSearchText 
        {
            get { return _piPointSearchText; }
            set { SetField<string>(ref _piPointSearchText, value); }
        }

        public ICommand PiPointSearchCommand { get; }

        private void OnPiPointSearch()
        {

        }
    }
}