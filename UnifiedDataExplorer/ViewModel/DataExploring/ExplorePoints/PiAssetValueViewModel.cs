using DotNetCommon.MVVM;
using PiModel;

namespace UnifiedDataExplorer.ViewModel.DataExploring.ExplorePoints
{
    public class PiAssetValueViewModel : ViewModelBase
    {

        private AssetValue _model;

        public PiAssetValueViewModel(AssetValue item)
        {
            _model = item;
            Name = _model.Name;
            Value = _model.Value.UntypedValue?.ToString();
        }
        public string Name { get; }

        public string Value { get; }

        public AssetValue GetBackingModel()
        {
            return _model;
        }
    }
}