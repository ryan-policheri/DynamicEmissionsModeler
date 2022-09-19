using System.Collections;
using DotNetCommon.Extensions;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;

namespace EmissionsMonitorServices
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
            ICollection<string> neededSeriesIds = spec.Model.GetAllSeriesIds();
            ICollection<Series> neededSeries = await QuerySeries(neededSeriesIds, spec.StartTime, spec.EndTime);

            ICollection<MonitorSeries> monitorSeriesList = new List<MonitorSeries>();

            bool first = true;
            foreach (DateTimeOffset offset in spec.StartTime.EnumerateSecondsUntil(spec.EndTime))
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
                        first = false;
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
            }

            return monitorSeriesList;
        }

        public async Task<ICollection<Series>> QuerySeries(ICollection<string> seriesIds, DateTimeOffset start, DateTimeOffset end)
        {
            ICollection<Series> seriesList = new List<Series>();
            foreach (string id in seriesIds)
            {
                Series series = await _dataSource.GetTimeSeriesAsync(id, start, end);
                seriesList.Add(series);
            }
            return seriesList;
        }
    }
}
