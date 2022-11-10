using DotNetCommon.Extensions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorDataAccess
{
    public class ModelExecutionService
    {
        private readonly ITimeSeriesDataSource _dataSource;

        public ModelExecutionService(ITimeSeriesDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<ICollection<MonitorSeries>> ExecuteModelAsync(ModelExecutionSpec spec)
        {
            IEnumerable<ProcessNode> leafs = spec.Model.GetProcessLeafs();
            ICollection<DataSourceSeriesUri> neededSeriesUris = spec.Model.GetAllSeriesUris();
            TimeSeriesRenderSettings renderSettings = new TimeSeriesRenderSettings
            {
                StartDateTime = spec.StartTime,
                EndDateTime = spec.EndTime,
                RenderResolution = spec.DataResolution
            };

            foreach (DataSourceSeriesUri seriesUri in neededSeriesUris.Where(x => x.SeriesDataResolution == DataResolutionPlusVariable.Variable))
            {
                seriesUri.FillVariableResolution(renderSettings.RenderResolution);
            }

            ICollection<Series> neededSeries = await QuerySeries(neededSeriesUris, renderSettings);

            ICollection<MonitorSeries> monitorSeriesList = new List<MonitorSeries>();

            bool first = true;
            foreach (DateTimeOffset offset in renderSettings.BuildTimePoints())
            {
                ICollection<DataPoint> timeData = neededSeries.SelectMany(x => x.DataPoints).Where(x => x.Timestamp == offset).ToList();

                foreach (ProcessNode leaf in leafs)
                {
                    if (first)
                    {
                        MonitorSeries monitorSeries = new MonitorSeries();
                        monitorSeries.SeriesName = leaf.Name;
                        monitorSeries.DataPoints = new List<MonitorDataPoint>();
                        monitorSeriesList.Add(monitorSeries);
                    }

                    MonitorSeries monitorSeries2 = monitorSeriesList.First(x => x.SeriesName == leaf.Name);
                    ProductCostResults results = leaf.RenderProductAndCosts(timeData);
                    MonitorDataPoint monitorPoint = new MonitorDataPoint
                    {
                        Timestamp = offset,
                        Values = results
                    };
                    monitorSeries2.DataPoints.Add(monitorPoint);
                }
                first = false;
            }

            return monitorSeriesList;
        }

        public async Task<ICollection<Series>> QuerySeries(ICollection<DataSourceSeriesUri> seriesUris, TimeSeriesRenderSettings renderSettings)
        {
            ICollection<Series> seriesList = new List<Series>();
            foreach (DataSourceSeriesUri uri in seriesUris)
            {
                Series series = await _dataSource.GetTimeSeriesAsync(uri, renderSettings);
                Series normalizedSeries = series.RenderSeriesAtTargetResolution(renderSettings);
                seriesList.Add(normalizedSeries);
            }
            return seriesList;
        }
    }
}
