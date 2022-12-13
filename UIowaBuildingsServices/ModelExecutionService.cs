using DotNetCommon.Extensions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.Exceptions;
using EmissionsMonitorModel.ProcessModeling;
using EmissionsMonitorModel.TimeSeries;
using System.Collections.Generic;
using System.Xml.Linq;

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

            IEnumerable<DataSourceSeriesUri> neededDataStreams = spec.Model.GetAllUniqueSeriesUris(null);
            foreach (DataSourceSeriesUri uri in neededDataStreams.Where(x => x.SeriesDataResolution == DataResolutionPlusVariable.Variable))
            {//All streams that are marked with variable resolution means that their owning system (I.E. PI) can render data at any resolution 
                uri.FillVariableResolution(spec.DataResolution); //So we say that the stream's resolution is whatever the user is looking for in this execution
            }
            ICollection<DataSourceSeriesUriQueryRender> queries = neededDataStreams //To convert the data streams to a query, we add the model execition info to it
                .Select(x => new DataSourceSeriesUriQueryRender(x, spec.StartTime, spec.EndTime, spec.DataResolution)).ToList();
            ICollection<Series> allSeries = await ExecuteAllQueries(queries); //Get all the data needed to render the entire model

            ICollection<NodeSeries> monitorSeriesList = new List<NodeSeries>();
            bool first = true;

            ICollection<DataPoint>? previousValidData = spec.OverflowHandleStrategy.HasValue && spec.OverflowHandleStrategy == OverflowHandleStrategies.UsePrevious ? new List<DataPoint>() : null;
            ICollection<NodeOverflowError> errors = spec.OverflowHandleStrategy.HasValue ? new List<NodeOverflowError>() : null;

            foreach (DateTimeOffset offset in DataResolution.BuildTimePoints(spec.DataResolution, spec.StartTime, spec.EndTime))
            {
                ICollection<DataPoint> timeData = allSeries.SelectMany(x => x.DataPoints).Where(x => x.Timestamp == offset).ToList();

                try
                {
                    foreach (ProcessNode node in nodes)
                    {
                        if (first)
                        {
                            NodeSeries newMonitorSeries = new NodeSeries();
                            newMonitorSeries.NodeId = node.Id;
                            newMonitorSeries.NodeName = node.Name;
                            newMonitorSeries.NodeOutputPoints = new List<NodeOutputPoint>();
                            monitorSeriesList.Add(newMonitorSeries);
                        }

                        ProductCostResults results = node.RenderProductAndCosts(timeData);
                        NodeOutputPoint monitorPoint = new NodeOutputPoint
                        {
                            Timestamp = offset,
                            Values = results
                        };

                        NodeSeries monitorSeries = monitorSeriesList.First(x => x.NodeId == node.Id);
                        monitorSeries.NodeOutputPoints.Add(monitorPoint);
                    }

                    previousValidData = timeData;
                }
                catch (NodeOverflowException ex)
                {
                    errors.Add(ex.Error);
                    if (spec.OverflowHandleStrategy.HasValue)
                    {
                        switch (spec.OverflowHandleStrategy)
                        {
                            case OverflowHandleStrategies.UsePrevious:
                                foreach (ProcessNode node in nodes)
                                {
                                    //Remove all points from timestamp that had issue
                                    NodeSeries monitorSeries = monitorSeriesList.First(x => x.NodeId == node.Id);
                                    var pointToRemove = monitorSeries.NodeOutputPoints.FirstOrDefault(x => x.Timestamp == offset);
                                    if(pointToRemove != null) monitorSeries.NodeOutputPoints.Remove(pointToRemove);

                                    //Use previous data to render this time stamp
                                    ProductCostResults results = node.RenderProductAndCosts(previousValidData);
                                    NodeOutputPoint monitorPoint = new NodeOutputPoint
                                    {
                                        Timestamp = offset,
                                        Values = results
                                    };
                                    monitorSeries.NodeOutputPoints.Add(monitorPoint);
                                }
                                break;
                            case OverflowHandleStrategies.ExcludeTimeslot:
                                break;
                            default: throw new NotImplementedException("Overflow strategy not implemented");
                        }
                    }
                    else throw;
                }
                first = false;
            }

            return new ModelExecutionResult
            {
                ExecutionSpec = spec,
                NodeSeries = monitorSeriesList,
                Errors = errors
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
