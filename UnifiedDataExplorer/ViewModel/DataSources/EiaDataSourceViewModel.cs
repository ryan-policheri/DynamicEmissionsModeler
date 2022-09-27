using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EIA.Services.Clients;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class EiaDataSourceViewModel : DataSourceBaseViewModel
    {
        public const string OPEN_EIA_EXPLORER = "OPEN_EIA_EXPLORER";
        private EiaDataSource _model;

        public EiaDataSourceViewModel(IDataSourceRepository repo, RobustViewModelDependencies facade) : base(repo, facade)
        {
            OpenEiaExplorer = new DelegateCommand(OnOpenEiaExplorer);
        }

        public ICommand OpenEiaExplorer { get; }

        public void Load(EiaDataSource model = null)
        {
            if (model == null) model = new EiaDataSource();
            _model = model;
            EiaBaseUrl = _model.SuggestedBaseUrl;
        }


        [Required]
        public string EiaBaseUrl
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
        public string EiaApiKey
        {
            get { return _model.SubscriptionKey; }
            set
            {
                _model.SubscriptionKey = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public override DataSourceBase GetBackingModel() => _model;

        protected override async Task<bool> TestDataSourceConnectionAsync()
        {
            EiaClient client = this.Resolve<EiaClient>();
            client.Initialize(_model.BaseUrl, _model.SubscriptionKey);
            await client.TestAsync();
            return true;
        }

        private void OnOpenEiaExplorer()
        {
            this.MessageHub.Publish<OpenViewModelEvent>(new OpenViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(EiaDataSourceViewModel),
                Verb = OPEN_EIA_EXPLORER
            });
        }
    }
}
