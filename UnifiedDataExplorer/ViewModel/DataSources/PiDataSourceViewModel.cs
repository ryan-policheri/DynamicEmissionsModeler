using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorServices.DataSourceWrappers;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class PiDataSourceViewModel : DataSourceBaseViewModel
    {
        public const string OPEN_AF_EXPLORER = "OPEN_PI_AF_EXPLORER";
        public const string OPEN_PI_SEARCH_EXPLORER = "OPEN_PI_SEARCH_EXPLORER";
        private PiDataSource _model;

        public PiDataSourceViewModel(IDataSourceRepository repo, DataSourceServiceFactory clientFactory, RobustViewModelDependencies facade) : base(repo, clientFactory, facade)
        {
            OpenAfExplorer = new DelegateCommand(OnOpenAfExplorer);
            OpenPiSearchExplorer = new DelegateCommand(OnOpenPiSearchExplorer);
        }

        [Required]
        public string PiWebApiUrl
        {
            get { return _model.BaseUrl; }
            set
            {
                _model.BaseUrl = value;
                OnPropertyChanged();
                Validate();
            }
        }

        [Required]
        public string PiDefaultAssetServer
        {
            get { return _model.DefaultAssetServer; }
            set
            {
                _model.DefaultAssetServer = value;
                OnPropertyChanged();
                Validate();
            }
        }

        [Required]
        public string PiUserName
        {
            get { return _model.UserName; }
            set
            {
                _model.UserName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        [Required]
        public string PiPassword
        {
            get { return _model.Password; }
            set
            {
                _model.Password = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public ICommand OpenAfExplorer { get; }

        public ICommand OpenPiSearchExplorer { get; }

        public void Load(PiDataSource model = null)
        {
            if (model == null) model = new PiDataSource();
            _model = model;
        }


        public override DataSourceBase GetBackingModel() => _model;

        protected override Task<bool> TestDataSourceConnectionAsync()
        {
            throw new System.NotImplementedException();
        }

        private void OnOpenAfExplorer()
        {
            this.MessageHub.Publish<OpenDataSourceViewModelEvent>(new OpenDataSourceViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(PiDataSourceViewModel),
                DataSourceId = this.DataSourceId,
                Verb = OPEN_AF_EXPLORER
            });
        }

        private void OnOpenPiSearchExplorer()
        {
            this.MessageHub.Publish<OpenDataSourceViewModelEvent>(new OpenDataSourceViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(PiDataSourceViewModel),
                DataSourceId = this.DataSourceId,
                Verb = OPEN_PI_SEARCH_EXPLORER
            });
        }
    }
}