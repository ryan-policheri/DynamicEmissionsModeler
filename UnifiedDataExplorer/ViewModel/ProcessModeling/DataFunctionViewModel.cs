using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class DataFunctionViewModel : ViewModelBase
    {
        private DataFunction _model;
        private readonly ICollection<FunctionTypeMapping> _functionTypes;

        public DataFunctionViewModel(DataFunction func)
        {
            _model = func;
            _functionTypes = DataFunction.GetAllFunctionTypeMappings().ToList();
            UnitTypes = new ObservableCollection<string>();
            UnitForms = new ObservableCollection<string>();
            foreach (string unitType in _functionTypes.Select(x => x.FunctionUnit).Distinct())
            {
                UnitTypes.Add(unitType);
            }
        }

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
                    OnPropertyChanged(nameof(NameReady));
                    if (String.IsNullOrWhiteSpace(FunctionName)) FunctionName = $"New {SelectedUnitForm} {SelectedUnitType} Function";
                }
            }
        }

        public bool NameReady => this._model != null;

        public string FunctionName
        {
            get { return _model?.FunctionName; }
            set
            {
                if (_model != null) _model.FunctionName = value; OnPropertyChanged();
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
