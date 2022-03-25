using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.PersistenceHelpers;
using PiModel;
using PiServices;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel
{
    public class PiSearchViewModel : RobustViewModelBase
    {
        private PiHttpClient _client;

        public const string SHOW_JSON = "SHOW_JSON";
        public const string RENDER_VALUES = "RENDER_VALUES";

        public PiSearchViewModel(PiHttpClient client, RobustViewModelDependencies facade) : base(facade)
        {
            _client = client;
            SearchItems = new ObservableCollection<PiSearchItemViewModel>();
            PiPointSearchCommand = new DelegateCommand(OnPiPointSearch);
            ClearSearchesCommand = new DelegateCommand(OnClearSearches);
            ViewJsonCommand = new DelegateCommand<PiSearchItemViewModel>(OnViewJson);
            RenderValuesCommand = new DelegateCommand<PiSearchItemViewModel>(OnRenderValues);
        }

        public string Header => "PI Search";
        public string HeaderDetail => "Search for PI tags";
        public bool IsCloseable => false;


        private string _piPointSearchText;
        public string PiPointSearchText 
        {
            get { return _piPointSearchText; }
            set { SetField<string>(ref _piPointSearchText, value); }
        }

        public bool _tagNotFound;
        public bool TagNotFound 
        {
            get { return _tagNotFound; }
            private set { SetField<bool>(ref _tagNotFound, value); }
        }

        public ObservableCollection<PiSearchItemViewModel> SearchItems { get; }

        public ICommand PiPointSearchCommand { get; }

        public ICommand ClearSearchesCommand { get; }

        public ICommand ViewJsonCommand { get; }

        public ICommand RenderValuesCommand { get; }

        public async Task LoadAsync()
        {
            AppDataFile dataFile = DataFileProvider.BuildSearchHistoryDataFile();
            if(dataFile.FileExists)
            {
                IEnumerable<PiSearchItemViewModel> items = await dataFile.ReadAsync<List<PiSearchItemViewModel>>();
                foreach (var item in items)
                {
                    SearchItems.Add(item);
                }
            }
        }

        private async void OnPiPointSearch()
        {
            if (String.IsNullOrWhiteSpace(PiPointSearchText)) return;
            PiPoint piPoint = await _client.SearchPiPoint(PiPointSearchText);
            if (piPoint == null) TagNotFound = true;
            else
            {
                TagNotFound = false;

                PiSearchItemViewModel existingItem = SearchItems.Where(x => x.TagName.CapsAndTrim() == piPoint.Name.CapsAndTrim()).FirstOrDefault();
                if (existingItem != null) SearchItems.Remove(existingItem);

                PiSearchItemViewModel viewModel = new PiSearchItemViewModel(piPoint);
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

        private void OnViewJson(PiSearchItemViewModel obj) => PublishOpenViewModelEvent(obj, SHOW_JSON);

        private void OnRenderValues(PiSearchItemViewModel obj) => PublishOpenViewModelEvent(obj, RENDER_VALUES);

        private void PublishOpenViewModelEvent(PiSearchItemViewModel obj, string action)
        {
            this.MessageHub.Publish<OpenViewModelEvent>(new OpenViewModelEvent
            {
                Sender = this,
                SenderTypeName = nameof(PiSearchViewModel),
                Id = obj.Link,
                Name = obj.TagName,
                Verb = action,
                TypeTag = PiPoint.PI_POINT_TYPE
            });
        }
    }
}