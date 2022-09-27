using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class DataSourceBaseViewModel : RobustViewModelBase
    {
        protected readonly IDataSourceRepository Repo;

        public DataSourceBaseViewModel(IDataSourceRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            Repo = repo;
            Save = new DelegateCommand(OnSave);
            TestConnection = new DelegateCommand(OnTestConnection);
            Cancel = new DelegateCommand(OnCancel);
        }

        [Required]
        public string SourceName
        {
            get { return GetBackingModel().SourceName; }
            set
            {
                GetBackingModel().SourceName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public ICommand Save { get; }

        public ICommand TestConnection { get; }

        public ICommand Cancel { get; }

        public DataSourceBaseViewModel InitializeSubclassViewModel(DataSourceType sourceType, DataSourceBase model = null)
        {
            switch (sourceType)
            {
                case DataSourceType.Eia:
                    var evm = this.Resolve<EiaDataSourceViewModel>();
                    evm.Load((EiaDataSource)model);
                    return evm;
                case DataSourceType.Pi:
                    var pvm = this.Resolve<PiDataSourceViewModel>();
                    pvm.Load((PiDataSource)model);
                    return pvm;
                default:
                    throw new NotImplementedException();
            }
        }

        public virtual DataSourceBase GetBackingModel() => throw new NotImplementedException();
        protected virtual Task<bool> TestDataSourceConnectionAsync() => throw new NotImplementedException();

        private async void OnSave()
        {
            if (this.IsValid())
            {
                DataSourceBase model = GetBackingModel();
                DataSourceBase dataSource = await Repo.SaveDataSource(model);
                this.MessageHub.Publish(new SaveViewModelEvent
                {
                    Sender = this,
                    SenderTypeName = this.GetType().Name
                });
            }
        }

        private async void OnTestConnection()
        {
            try
            {
                bool success = await this.TestDataSourceConnectionAsync();
                if(success) this.DialogService.ShowInfoMessage("Connection successful!");
                else this.DialogService.ShowErrorMessage("Error: Connection not successful");
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
                SenderTypeName = this.GetType().Name
            });
        }
    }
}