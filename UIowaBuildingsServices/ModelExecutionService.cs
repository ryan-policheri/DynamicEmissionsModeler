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

        public async Task<ModelExecutionResult> ExecuteModelAsync(ModelExecutionSpec spec)
        {
            //Render each node of the model across all time points
            IEnumerable<ProcessNode> nodes = spec.NodeIds == null
                ? spec.Model.GetAllNodes(true)
                : spec.Model.GetAllNodes(true).Where(x => spec.NodeIds.Any(y => y == x.Id));

            IEnumerable<DataSourceSeriesUri> neededDataStreams = spec.Model.GetAllUniqueSeriesUris(spec.NodeIds);
            foreach (DataSourceSeriesUri uri in neededDataStreams.Where(x => x.SeriesDataResolution == DataResolutionPlusVariable.Variable))
            {//All streams that are marked with variable resolution means that their owning system (I.E. PI) can render data at any resolution 
                uri.FillVariableResolution(spec.DataResolution); //So we say that the stream's resolution is whatever the user is looking for in this execution
            }
            ICollection<DataSourceSeriesUriQueryRender> queries = neededDataStreams //To convert the data streams to a query, we add the model execition info to it
                .Select(x => new DataSourceSeriesUriQueryRender(x, spec.StartTime, spec.EndTime, spec.DataResolution)).ToList();
            ICollection<Series> allSeries = await ExecuteAllQueries(queries); //Get all the data needed to render the entire model

            ICollection<MonitorSeries> monitorSeriesList = new List<MonitorSeries>();
            bool first = true;

            foreach (DateTimeOffset offset in DataResolution.BuildTimePoints(spec.DataResolution, spec.StartTime, spec.EndTime))
            {
                ICollection<DataPoint> timeData = allSeries.SelectMany(x => x.DataPoints).Where(x => x.Timestamp == offset).ToList();

                foreach (ProcessNode node in nodes)
                {
                    if (first)
                    {
                        MonitorSeries monitorSeries = new MonitorSeries();
                        monitorSeries.SeriesName = node.Name;
                        monitorSeries.DataPoints = new List<MonitorDataPoint>();
                        monitorSeriesList.Add(monitorSeries);
                    }

                    MonitorSeries monitorSeries2 = monitorSeriesList.First(x => x.SeriesName == node.Name);
                    ProductCostResults results = node.RenderProductAndCosts(timeData);
                    MonitorDataPoint monitorPoint = new MonitorDataPoint
                    {
                        Timestamp = offset,
                        Values = results
                    };
                    monitorSeries2.DataPoints.Add(monitorPoint);
                }
                first = false;
            }

            return new ModelExecutionResult
            {
                ExecutionSpec = spec,
                NodeSeries = monitorSeriesList
            };
        }

        private async Task<ICollection<Series>> ExecuteAllQueries(ICollection<DataSourceSeriesUriQueryRender> queries)
        {
            ICollection<Series> seriesList = new List<Series>();
            foreach (DataSourceSeriesUriQueryRender query in queries)
            {
                Series series = await _dataSource.GetTimeSeriesAsync(query);
                Series normalizedSeries = series.RenderSeriesAtTargetResolution(query.GetQueryRenderSettings());
                seriesList.Add(normalizedSeries);
            }
            return seriesList;
        }
    }
}
