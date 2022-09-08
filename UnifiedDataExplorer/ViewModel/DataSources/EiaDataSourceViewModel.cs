using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EIA.Services.Clients;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class EiaDataSourceViewModel : RobustViewModelBase
    {
        public EiaDataSource _model;

        public EiaDataSourceViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            Save = new DelegateCommand(OnSave);
            TestConnection = new DelegateCommand(OnTestConnection);
            Cancel = new DelegateCommand(OnCancel);
        }

        public async Task LoadAsync(EiaDataSource model = null)
        {
            if (model == null) model = new EiaDataSource();
            _model = model;
            EiaBaseUrl = _model.SuggestedBaseUrl;
        }

        public string EiaBaseUrl
        {
            get { return _model.BaseUrl; }
            set
            {
                _model.BaseUrl = value;
                OnPropertyChanged(nameof(EiaBaseUrl));
            }
        }


        public string EiaApiKey
        {
            get { return _model.SubscriptionKey; }
            set
            {
                _model.SubscriptionKey = value;
                OnPropertyChanged(nameof(EiaApiKey));
            }
        }

        public ICommand Save { get; }

        public ICommand TestConnection { get; }

        public ICommand Cancel { get; }

        private void OnSave()
        {
            //TODO: SAVE
            this.MessageHub.Publish(new SaveViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(EiaDataSourceViewModel)
            });
        }

        private async void OnTestConnection()
        {
            EiaClient client = this.Resolve<EiaClient>();
            try
            {
                client.Initialize(_model.BaseUrl, _model.SubscriptionKey);
                await client.TestAsync();
                this.DialogService.ShowInfoMessage("Connection successful!");
            }
            catch (Exception ex)
            {
                this.DialogService.ShowErrorMessage(ex.Message);
            }
        }

        private void OnCancel()
        {
            this.MessageHub.Publish(new CloseViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(EiaDataSourceViewModel)
            });
        }
    }
}
