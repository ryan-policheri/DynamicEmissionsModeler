using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.VirtualFileSystem;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataExploring
{
    public class ExploreSetsNavigationViewModel : RobustViewModelBase
    {
        private readonly IVirtualFileSystemRepository _repo;

        public ExploreSetsNavigationViewModel(IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            _repo = repo;
            Folders = new ObservableCollection<Folder>();
        }

        public ObservableCollection<Folder> Folders { get; }

        public async Task LoadAsync()
        {
            var roots = await _repo.GetAllRootFoldersAsync();
            var exploreSetRoot = roots.FirstOrDefault(x => x.FolderName == SystemRoots.EXPLORE_SETS);
            if(exploreSetRoot == null) exploreSetRoot = await _repo.CreateRootFolderAsync(new Folder{ FolderName = SystemRoots.EXPLORE_SETS});

            var root = await _repo.GetFolderRecursiveAsync(exploreSetRoot.FolderId);
            Folders.Add(root);
        }
    }
}
