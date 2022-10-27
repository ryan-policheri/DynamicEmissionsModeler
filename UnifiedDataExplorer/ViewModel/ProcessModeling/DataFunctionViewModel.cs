using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.DynamicCompilation;
using DotNetCommon.Extensions;
using EmissionsMonitorDataAccess;
using EmissionsMonitorModel.ProcessModeling;
using Microsoft.CSharp.RuntimeBinder;
using UnifiedDataExplorer.Services.WindowDialog;
using UnifiedDataExplorer.ViewModel.Base;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class DataFunctionViewModel : RobustViewModelBase
    {
        private readonly ModelInitializationService _compilationService;
        private readonly ICollection<FunctionTypeMapping> _functionTypes;
        private DataFunction _model;
        private Action<ViewModelDataStatus> _onDoneCallback;

        public DataFunctionViewModel(ModelInitializationService compilationService, RobustViewModelDependencies facade) : base(facade)
        {
            _compilationService = compilationService;
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
            CompileCommand = new DelegateCommand(OnCompileCommand);
            TestCommand = new DelegateCommand(OnTest);
            DoneCommand = new DelegateCommand<ViewModelDataStatus?>(OnDoneCommand);
        }

        public void Load(DataFunction func, Action<ViewModelDataStatus> callback)
        {
            if (func != null)
            {
                IsInModel = true;
                SelectedUnitType = UnitTypes.First(x => x == func.FunctionUnit);
                SelectedUnitForm = UnitForms.First(x => x == func.FunctionUnitForm);
                foreach (FunctionFactor factor in func.FunctionFactors)
                {
                    FunctionFactorViewModel vm = new FunctionFactorViewModel(factor);
                    this.FunctionFactors.Add(vm);
                }
            }
            _model = func;
            _onDoneCallback = callback;
            ShowDoneOptions = _onDoneCallback != null;
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

        public string FunctionDescription
        {
            get { return _model?.FunctionDescription; }
            set { if (_model != null) _model.FunctionDescription = value; OnPropertyChanged(); }
        }

        public string ReturnType => _model?.GetReturnType().Name;

        public string MethodName => _model?.FunctionName.ToValidMethodName();

        public string FunctionCode
        {
            get { return _model?.FunctionCode; }
            set { if(_model != null) _model.FunctionCode = value; OnPropertyChanged(); }
        }

        public ICommand CompileCommand { get; }

        public ICommand TestCommand { get; }

        private async void OnCompileCommand()
        {
            await CompileCode(true);
        }

        private async Task CompileCode(bool showSucessMessage = true)
        {
            try
            {
                await _compilationService.InitializeFunction(this._model);
                if(showSucessMessage) DialogService.ShowInfoMessage("Function compiled successfully!");
            }
            catch (RuntimeBinderInternalCompilerException ex) //TODO better exception
            {
                DialogService.ShowErrorMessage(ex.Message);
                _model.FunctionHostObject = null;
            }
        }

        private async void OnTest()
        {
            await CompileCode(false);
            if (_model.FunctionHostObject == null) return;
            DynamicTestingViewModel vm = new DynamicTestingViewModel(_model);
            DialogService.ShowModalWindow(vm, null, ModalOptions.DefaultModalOptions);
        }

        private bool _showDoneOptions;
        public bool ShowDoneOptions
        {
            get { return _showDoneOptions; }
            set { SetField(ref _showDoneOptions, value); }
        }

        public ICommand DoneCommand { get; }
        private void OnDoneCommand(ViewModelDataStatus? obj)
        {
            if (obj != null) _onDoneCallback(obj.Value);
        }

        private void PopulateUnitForms(string selectedUnit)
        {
            this.UnitForms.Clear();
            foreach (string form in _functionTypes.Where(x => x.FunctionUnit == selectedUnit).Select(x => x.FunctionUnitForm))
            {
                this.UnitForms.Add(form);
            }
        }

        public ObservableCollection<FunctionFactorViewModel> FunctionFactors { get; }

        public ICommand OpenFactor { get; }
        private void OnOpenFactor(FunctionFactorViewModel obj) => OnFactorSelected(obj);

        public ICommand AddFactor { get; }
        private void OnAddFactor() => OnFactorSelected(null);

        private void OnFactorSelected(FunctionFactorViewModel factorVm)
        {
            bool isNew = false;
            if (factorVm == null)
            {
                isNew = true;
                FunctionFactor backingModel = new FunctionFactor();
                factorVm = new FunctionFactorViewModel(backingModel);
            }

            FunctionFactor factor = factorVm.GetBackingModel();
            FunctionFactor factorCopy = factor.Copy();

            ModalOptions options = ModalOptions.SaveCancelOption; if (!isNew) { options.ShowDelete = true; }
            
            this.DialogService.ShowModalWindow(factorVm, () =>
            {
                if (factorVm.Status == ViewModelDataStatus.Saved)
                {
                    if(isNew) { _model.FunctionFactors.Add(factor); this.FunctionFactors.Add(factorVm); }
                    else { /*do nothing*/ }
                }
                if (factorVm.Status == ViewModelDataStatus.Deleted) { this._model.FunctionFactors.Remove(factor); this.FunctionFactors.Remove(factorVm); }
                if (factorVm.Status == ViewModelDataStatus.Canceled)
                {
                    if(isNew) { /*do nothing*/ }
                    else 
                    { 
                        _model.FunctionFactors.Remove(factor);
                        _model.FunctionFactors.Add(factorCopy);
                        this.FunctionFactors.Remove(factorVm);
                        this.FunctionFactors.Add(new FunctionFactorViewModel(factorCopy));
                    }
                }
            }, options);
        }
    }
}
