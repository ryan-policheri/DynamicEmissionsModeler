using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using DotNetCommon.DelegateCommand;
using DotNetCommon.MVVM;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;

namespace UnifiedDataExplorer.ViewModel.ProcessModeling
{
    public class DynamicTestingViewModel : ViewModelBase
    {
        private readonly DataFunction _dataFunc;

        public DynamicTestingViewModel(DataFunction dataFunction)
        {
            _dataFunc = dataFunction;
            AllSeries = new ObservableCollection<Series>();

            foreach (var factor in dataFunction.FunctionFactors)
            {
                Series series = new Series { SeriesUri = factor.FactorUri };
                series.DataPoints = new List<DataPoint>();
                AllSeries.Add(series);
            }

            ColumnCount = 1;
            ExecuteCommand = new DelegateCommand(OnExecute);
        }

        public ObservableCollection<Series> AllSeries { get; }

        private int _columnCount;
        public int ColumnCount
        {
            get { return _columnCount; }
            set
            {
                if (value >= 1 && value != _columnCount)
                {
                    AdjustColumns(value);
                }
            }
        }

        public ICommand ExecuteCommand { get; }

        private Series _outputSeries;
        public Series OutputSeries
        {
            get { return _outputSeries; }
            set { SetField(ref _outputSeries, value);}
        }


        private void AdjustColumns(int newColumnCount)
        {
            List<Series> newSeries = new List<Series>();
            foreach (Series series in AllSeries) { newSeries.Add(series); }

            foreach (Series series in newSeries)
            {
                if (series.DataPoints.Count() > newColumnCount)
                {
                    series.DataPoints = series.DataPoints.Take(newColumnCount).ToList();
                }
                else if (series.DataPoints.Count() < newColumnCount)
                {
                    List<DataPoint> newList = series.DataPoints.ToList();
                    for (int i = newList.Count; i < newColumnCount; i++)
                    {
                        newList.Add(new DataPoint { Series = series, Value = 0 });
                    }
                    series.DataPoints = newList;
                }
            }

            AllSeries.Clear();
            foreach (Series series in newSeries) { AllSeries.Add(series); }

            _columnCount = newColumnCount;
            OnPropertyChanged(nameof(ColumnCount));
        }

        private void OnExecute()
        {
            var output = new Series()
            {
                DataPoints = new List<DataPoint>()
            };

            for (int i = 0; i < _columnCount; i++)
            {
                ICollection<DataPoint> points = new List<DataPoint>();
                foreach (Series series in AllSeries)
                {
                    points.Add(series.DataPoints.ElementAt(i));
                }

                var result = _dataFunc.ExecuteFunction(points);
                output.DataPoints.Add(new DataPoint
                {
                    Value = result.TotalValue
                });
            }

            OutputSeries = output;
        }
    }
}
