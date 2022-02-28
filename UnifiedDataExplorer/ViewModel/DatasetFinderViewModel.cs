using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DotNetCommon.EventAggregation;
using DotNetCommon.MVVM;
using EIA.Domain.Constants;
using EIA.Domain.Model;
using EIA.Services.Clients;
using EIADataViewer.Events;
using EIADataViewer.ModelWrappers;

namespace EIADataViewer.ViewModel
{
    public class DatasetFinderViewModel : ViewModelBase
    {
        private readonly EiaClient _client;
        private readonly IMessageHub _messageHub;

        public DatasetFinderViewModel(EiaClient client, IMessageHub messageHub)
        {
            _client = client;
            _categories = new ObservableCollection<LazyTreeItemViewModel>();
            _messageHub = messageHub;
        }

        public string Header => "Dataset Finder";
        public string HeaderDetail => "Navigate to a EIA dataset";
        public bool IsCloseable => false;

        private ObservableCollection<LazyTreeItemViewModel> _categories;
        public ObservableCollection<LazyTreeItemViewModel> Categories
        {
            get { return _categories; }
            private set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public async Task LoadAsync()
        {
            Category root = await _client.GetCategoryByIdAsync(EiaCategories.ABSOLUTE_ROOT);
            CategorySeriesWrapper wrapper = new CategorySeriesWrapper(root);
            Categories.Add(new LazyTreeItemViewModel(wrapper));
        }

        public async Task LoadChildrenAsync(LazyTreeItemViewModel treeItem)
        {
            if (!treeItem.ChildrenLoaded)
            {
                ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
                CategorySeriesWrapper model = modelInterface as CategorySeriesWrapper;

                if(model != null && model.IsCategory())
                {
                    Category itemToLoad = await _client.GetCategoryByIdAsync(int.Parse(treeItem.Id));
                    CategorySeriesWrapper wrappedItem = new CategorySeriesWrapper(itemToLoad);
                    treeItem.AppendLoadedChildren(wrappedItem.Children);
                }
            }
        }

        public async Task PeformLeafActionAsync(LazyTreeItemViewModel treeItem)
        {
            ILazyTreeItemBackingModel modelInterface = treeItem.GetBackingModel();
            CategorySeriesWrapper model = modelInterface as CategorySeriesWrapper;
            if (model != null && model.IsSeries())
            {
                _messageHub.Publish<OpenViewModelEvent>(new OpenViewModelEvent { Sender = this, SenderTypeName = nameof(DatasetFinderViewModel), Id = model.GetId() });
                return;
            }
        }
    }
}
