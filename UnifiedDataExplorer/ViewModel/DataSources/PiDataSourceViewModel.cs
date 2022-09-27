using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataSources
{
    public class PiDataSourceViewModel : DataSourceBaseViewModel
    {
        private PiDataSource _model;

        public PiDataSourceViewModel(IDataSourceRepository repo, RobustViewModelDependencies facade) : base(repo, facade)
        {
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
    }
}