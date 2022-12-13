using DotNetCommon.DelegateCommand;
using System.Windows.Input;
using UnifiedDataExplorer.Events;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.DataExploring.Explorers
{
    public abstract class ExplorerViewModel : RobustViewModelBase
    {
        public ExplorerViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            CloseExplorationItemCommand = new DelegateCommand(OnCloseExplorationItem);
        }

        public abstract string Header { get; }

        public abstract string HeaderDetail { get; }

        public bool IsCloseable => true;

        public ICommand CloseExplorationItemCommand { get; }

        private void OnCloseExplorationItem()
        {
            MessageHub.Publish(new CloseViewModelEvent { Sender = this, SenderTypeName = nameof(ExplorerViewModel) });
        }
    }
}
