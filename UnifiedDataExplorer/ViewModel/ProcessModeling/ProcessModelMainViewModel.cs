﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.Extensions;
using DotNetCommon.MVVM;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.VirtualFileSystem;
using UnifiedDataExplorer.ViewModel.Base;
using UnifiedDataExplorer.ViewModel.VirtualFileSystem;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class ProcessModelMainViewModel : RobustViewModelBase
    {
        private readonly ProcessNodesViewModel _processNodesVm;
        private readonly IVirtualFileSystemRepository _repo;
        private ModelSaveItem _saveItem;
        private ProcessModel _model;

        public ProcessModelMainViewModel(ProcessNodesViewModel processNodesVm, IVirtualFileSystemRepository repo, RobustViewModelDependencies facade) : base(facade)
        {
            _processNodesVm = processNodesVm;
            _repo = repo;
            SaveCommand = new DelegateCommand<bool?>(OnSaveCommand);
            ExecuteCommand = new DelegateCommand<bool?>(OnExecuteCommand);
            ViewProcessNodesCommand = new DelegateCommand(() => this.CurrentChild = processNodesVm);
            ViewDataStreamsCommand = new DelegateCommand(() => throw new NotImplementedException());
        }

        public async Task LoadAsync(int id = -1)
        {
            if (id > 0)
            {
                _saveItem = await _repo.GetModelSaveItemAsync(id);
                _model = _saveItem.ToProcessModel();
            }
            else _model = new ProcessModel();

            _processNodesVm.Load(_model);
            CurrentChild = _processNodesVm;
        }

        private ViewModelBase _currentChild;
        public ViewModelBase CurrentChild
        {
            get { return _currentChild; }
            private set { SetField(ref _currentChild, value); }
        }

        public ICommand SaveCommand { get; }
        private async void OnSaveCommand(bool? saveAs)
        {
            await SaveAsync(saveAs.Value);
        }
        private async Task SaveAsync(bool saveAs)
        {
            if (_saveItem == null || saveAs)
            {
                var vm = this.Resolve<EnergyModelFileSystemViewModel>();
                _saveItem = new ModelSaveItem { ProcessModelJsonDetails = _model.ToJson<ProcessModel>() };
                vm.SaveData = _saveItem;
                await vm.LoadAsync(FileSystemMode.SaveOrManage);
                this.DialogService.ShowModalWindow(vm);
            }
            else
            {
                _saveItem.ProcessModelJsonDetails = _model.ToJson<ProcessModel>();
                _saveItem = await _repo.SaveModelSaveItemAsync(_saveItem);
            }
        }

        public ICommand ExecuteCommand { get; }
        private async void OnExecuteCommand(bool? live)
        {
            await ExecuteAsync(live.Value);
        }
        private async Task ExecuteAsync(bool live)
        {
            await SaveAsync(false);
            if (live)
            {
                ExecutionViewModel vm = this.Resolve<ExecutionViewModel>();
                vm.Load(_saveItem.SaveItemId);
                this.CurrentChild = vm;
            }
            else
            {

            }
        }

        public ICommand ViewProcessNodesCommand { get; }

        public ICommand ViewDataStreamsCommand { get; }
    }
}
