using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.PersistenceHelpers;
using EmissionsMonitorServices.DataSourceWrappers;
using PiModel;
using PiServices;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataExploring.Explorers
{
    public class PiTagExplorerViewModel : ExplorerViewModel
    {
        private readonly DataSourceServiceFactory _clientFactory;
        private PiHttpClient _client;

        public const string SHOW_JSON = "SHOW_JSON";
        public const string RENDER_VALUES = "RENDER_VALUES";

        public PiTagExplorerViewModel(DataSourceServiceFactory clientFactory, RobustViewModelDependencies facade) : base(facade)
        {
            _clientFactory = clientFactory;
            SearchItems = new ObservableCollection<PiTagItemViewModel>();
            PiPointSearchCommand = new DelegateCommand(OnPiPointSearch);
            ClearSearchesCommand = new DelegateCommand(OnClearSearches);
            ViewJsonCommand = new DelegateCommand<PiTagItemViewModel>(OnViewJson);
            RenderValuesCommand = new DelegateCommand<PiTagItemViewModel>(OnRenderValues);
        }

        public override string Header => "PI Tag Explorer";
        public override string HeaderDetail => "Search for PI tags";


        private string _piPointSearchText;
        public string PiPointSearchText
        {
            get { return _piPointSearchText; }
            set { SetField(ref _piPointSearchText, value); }
        }

        public bool _tagNotFound;
        public bool TagNotFound
        {
            get { return _tagNotFound; }
            private set { SetField(ref _tagNotFound, value); }
        }

        public ObservableCollection<PiTagItemViewModel> SearchItems { get; }

        public ICommand PiPointSearchCommand { get; }

        public ICommand ClearSearchesCommand { get; }

        public ICommand ViewJsonCommand { get; }

        public ICommand RenderValuesCommand { get; }

        public async Task LoadAsync(int dataSourceId)
        {
            _client = _clientFactory.GetDataSourceServiceById<PiHttpClient>(dataSourceId);
            AppDataFile dataFile = DataFileProvider.BuildSearchHistoryDataFile();
            if (dataFile.FileExists)
            {
                IEnumerable<PiTagItemViewModel> items = await dataFile.ReadAsync<List<PiTagItemViewModel>>();
                foreach (var item in items)
                {
                    SearchItems.Add(item);
                }
            }
        }

        private async void OnPiPointSearch()
        {
            if (string.IsNullOrWhiteSpace(PiPointSearchText)) return;
            PiPoint piPoint = await _client.SearchPiPoint(PiPointSearchText);
            if (piPoint == null) TagNotFound = true;
            else
            {
                TagNotFound = false;

                PiTagItemViewModel existingItem = SearchItems.Where(x => x.TagName.CapsAndTrim() == piPoint.Name.CapsAndTrim()).FirstOrDefault();
                if (existingItem != null) SearchItems.Remove(existingItem);

                PiTagItemViewModel viewModel = new PiTagItemViewModel(piPoint);
                SearchItems.Insert(0, viewModel);

                AppDataFile dataFile = DataFileProvider.BuildSearchHistoryDataFile();
                dataFile.Save(SearchItems);
            }
        }

        private void OnClearSearches()
        {
            SearchItems.Clear();
            AppDataFile dataFile = DataFileProvider.BuildSearchHistoryDataFile();
            dataFile.Save(SearchItems);
        }

        private void OnViewJson(PiTagItemViewModel obj) => PublishOpenViewModelEvent(obj, SHOW_JSON);

        private void OnRenderValues(PiTagItemViewModel obj) => PublishOpenViewModelEvent(obj, RENDER_VALUES);

        private void PublishOpenViewModelEvent(PiTagItemViewModel obj, string action)
        {
            MessageHub.Publish(new OpenViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(PiTagExplorerViewModel),
                Id = obj.Link,
                Name = obj.TagName,
                Verb = action,
                TypeTag = PiPoint.PI_POINT_TYPE
            });
        }
    }
}