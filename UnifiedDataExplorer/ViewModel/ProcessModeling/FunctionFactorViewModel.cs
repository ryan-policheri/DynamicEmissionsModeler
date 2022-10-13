using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.ProcessModeling;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class FunctionFactorViewModel : ViewModelBase
    {
        private readonly FunctionFactor _model;

        public FunctionFactorViewModel(FunctionFactor factor)
        {
            _model = factor;
        }

        public string FactorName
        {
            get { return _model?.FactorName; }
            set { if(_model != null) _model.FactorName = value; OnPropertyChanged(); }
        }

    }
}
