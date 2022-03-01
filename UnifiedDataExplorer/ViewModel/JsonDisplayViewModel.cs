using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using DotNetCommon.Extensions;
using PiModel;

namespace UnifiedDataExplorer.ViewModel
{
    public class JsonDisplayViewModel : ExplorePointViewModel
    {
        private readonly IMessageHub _messageHub;

        public JsonDisplayViewModel(IMessageHub messageHub) : base(messageHub)
        {
            _messageHub = messageHub;
        }

        public string Json { get; private set; }

        public async Task LoadAsync(ItemBase itemBase)
        {
            Json = itemBase.ToBeautifulJson();
            Header = itemBase.Name.First(25) + " (Json)";
            HeaderDetail = itemBase.Name + " (Json)";
        }
    }
}