using DotNetCommon.MVVM;
using PiModel;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiAssetValueViewModel : ViewModelBase
    {
        public PiAssetValueViewModel(AssetValue item)
        {
            Name = item.Name;
            Value = item.Value.UntypedValue?.ToString();
        }

        public string Name { get; }

        public string Value { get; }
    }
}
