﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.DynamicCompilation;
using DotNetCommon.Extensions;
using EmissionsMonitorModel.ProcessModeling;
using UnifiedDataExplorer.Services.WindowDialog;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class DataFunctionViewModel : RobustViewModelBase
    {
        private readonly ICollection<FunctionTypeMapping> _functionTypes;
        private DataFunction _model;
        private Action<ViewModelDataStatus> _onDoneCallback;

        public DataFunctionViewModel(RobustViewModelDependencies facade) : base(facade)
        {
            _functionTypes = DataFunction.GetAllFunctionTypeMappings().ToList();
            UnitTypes = new ObservableCollection<string>();
            UnitForms = new ObservableCollection<string>();
            foreach (string unitType in _functionTypes.Select(x => x.FunctionUnit).Distinct())
            {
                UnitTypes.Add(unitType);
            }

            FunctionFactors = new ObservableCollection<FunctionFactorViewModel>();
            OpenFactor = new DelegateCommand<FunctionFactorViewModel>(OnOpenFactor);
            AddFactor = new DelegateCommand(OnAddFactor);
            DoneCommand = new DelegateCommand<ViewModelDataStatus?>(OnDoneCommand);
        }

        public void Load(DataFunction func, Action<ViewModelDataStatus> callback)
        {
            if (func != null)
            {
                IsInModel = true;
                SelectedUnitType = UnitTypes.First(x => x == func.FunctionUnit);
                SelectedUnitForm = UnitForms.First(x => x == func.FunctionUnitForm);
            }
            _model = func;
            _onDoneCallback = callback;
        }

        public DataFunction GetBackingModel() => _model;

        private bool _isInModel;
        public bool IsInModel
        {
            get { return _isInModel; }
            set { SetField(ref _isInModel, value); OnPropertyChanged(nameof(IsNew)); }
        }

        public bool IsNew => !IsInModel;

        public ObservableCollection<string> UnitTypes { get; }

        private string _selectedUnitType;
        public string SelectedUnitType
        {
            get { return _selectedUnitType; }
            set
            {
                SetField(ref _selectedUnitType, value);
                PopulateUnitForms(_selectedUnitType);
            }
        }

        public ObservableCollection<string> UnitForms { get; }

        private string _selectedUnitForm;
        public string SelectedUnitForm
        {
            get { return _selectedUnitForm; }
            set
            {
                SetField(ref _selectedUnitForm, value);
                if (_selectedUnitForm != null)
                {
                    _model = (DataFunction)Activator.CreateInstance(_functionTypes.Where(x => x.FunctionUnit == SelectedUnitType && x.FunctionUnitForm == _selectedUnitForm).First().TypeRep);
                    if (String.IsNullOrWhiteSpace(FunctionName)) FunctionName = $"New {SelectedUnitForm} {SelectedUnitType} Function";
                    OnPropertyChanged(nameof(UnitAndTypeSelected));
                    OnPropertyChanged(nameof(ReturnType));
                    OnPropertyChanged(nameof(MethodName));
                }
            }
        }

        public bool UnitAndTypeSelected => this._model != null;

        public string FunctionName
        {
            get { return _model?.FunctionName; }
            set { if (_model != null) _model.FunctionName = value; OnPropertyChanged(); OnPropertyChanged(nameof(MethodName)); }
        }

        public string ReturnType => _model?.GetReturnType().Name;

        public string MethodName => _model?.FunctionName.ToValidMethodName();

        public string FunctionCode
        {
            get { return _model?.FunctionCode; }
            set { if(_model != null) _model.FunctionCode = value; OnPropertyChanged(); }
        }

        public ObservableCollection<FunctionFactorViewModel> FunctionFactors { get; }

        public ICommand OpenFactor { get; }
        private void OnOpenFactor(FunctionFactorViewModel obj)
        {
            FunctionFactorViewModel copy = obj.Copy();
            this.DialogService.ShowModalWindow(obj, () =>
            {
                if (obj.Status == ViewModelDataStatus.Deleted) FunctionFactors.Remove(obj);
                if (obj.Status == ViewModelDataStatus.Canceled)
                {
                    FunctionFactors.Remove(obj);
                    FunctionFactors.Add(copy);
                }
            }, ModalOptions.SaveCancelDeleteOption);
        }

        public ICommand AddFactor { get; }
        private void OnAddFactor()
        {
            FunctionFactorViewModel factorVm = new FunctionFactorViewModel(new FunctionFactor());
            this.DialogService.ShowModalWindow(factorVm, () =>
            {
                if (factorVm.Status == ViewModelDataStatus.Saved) FunctionFactors.Add(factorVm);
            }, ModalOptions.SaveCancelOption);
        }

        public ICommand DoneCommand { get; }
        private void OnDoneCommand(ViewModelDataStatus? obj)
        {
            if (obj != null)
            {
                _onDoneCallback(obj.Value);
            }
        }

        private void PopulateUnitForms(string selectedUnit)
        {
            this.UnitForms.Clear();
            foreach (string form in _functionTypes.Where(x => x.FunctionUnit == selectedUnit)
                         .Select(x => x.FunctionUnitForm))
            {
                this.UnitForms.Add(form);
            }
        }
    }
}
