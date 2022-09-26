using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EIA.Services.Clients;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class EiaDataSourceViewModel : DataSourceBaseViewModel
    {
        private EiaDataSource _model;

        public EiaDataSourceViewModel(IDataSourceRepository repo, RobustViewModelDependencies facade) : base(repo, facade)
        {
        }

        public async Task LoadAsync(EiaDataSource model = null)
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

        protected override DataSourceBase GetBackingModel() => _model;

        protected override async Task<bool> TestDataSourceConnectionAsync()
        {
            EiaClient client = this.Resolve<EiaClient>();
            client.Initialize(_model.BaseUrl, _model.SubscriptionKey);
            await client.TestAsync();
            return true;
        }
    }
}
