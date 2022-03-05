using System;
using System.Data;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using PiModel;
using PiServices;
using UnifiedDataExplorer.ModelWrappers;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiInterpolatedDataViewModel : ExplorePointViewModel
    {
        private readonly PiHttpClient _client;

        public PiInterpolatedDataViewModel(PiHttpClient client, IMessageHub messageHub) : base(messageHub)
        {
            _client = client;
        }

        private string _messge;
        public string Message
        {
            get { return _messge; }
            set { SetField<string>(ref _messge, value); OnPropertyChanged(nameof(HasMessage)); }
        }

        public bool HasMessage => !String.IsNullOrWhiteSpace(this.Message);

        private DataTable _dataSet;
        public DataTable DataSet
        {
            get { return _dataSet; }
            private set
            {
                _dataSet = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync(IPiDetailLoadingInfo loadingInfo)
        {
            if (loadingInfo.Tag == nameof(AssetValue))
            {
                AssetValue value = await this._client.GetByDirectLink<AssetValue>(loadingInfo.Id);
                await this._client.LoadInterpolatedValues(value);
                DataSet = value.RenderDataPointsAsTable();
            }
            else
            {
                Header = "Error";
                HeaderDetail = $"Did not know how to render value for a {loadingInfo.Tag}";
                throw new ArgumentException(nameof(PiAssetValuesViewModel) + " can only render values of an asset");
            }
        }
    }
}
