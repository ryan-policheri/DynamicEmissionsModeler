using EmissionsMonitorModel.DataSources;
using EmissionsMonitorDataAccess.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EmissionsMonitorWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataSourceController : ControllerBase
    {
        private readonly IDataSourceRepository _repo;
        private readonly ILogger<DataSourceController> _logger;

        public DataSourceController(IDataSourceRepository repo, ILogger<DataSourceController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDataSources()
        {
            try
            {
                // Api should never return sensitive credentials
                IEnumerable<DataSourceBase> result = await _repo.GetAllDataSourcesAsync();
                List<DataSourceBase> dataSources = result.ToList();
                List<DataSourceBase> dataSourceSafe = new List<DataSourceBase>();
                foreach (DataSourceBase dataSource in dataSources)
                {
                    switch (dataSource.SourceType)
                    {
                        case DataSourceType.Pi:
                            PiDataSource piDataSource = (PiDataSource)dataSource;
                            piDataSource.Password = "";
                            piDataSource.SourceDetailsJson = piDataSource.ToSourceDetails();
                            dataSourceSafe.Add(piDataSource);
                            break;
                        case DataSourceType.Eia:
                            EiaDataSource eiaDataSource = (EiaDataSource)dataSource;
                            eiaDataSource.SubscriptionKey = "";
                            eiaDataSource.SourceDetailsJson = eiaDataSource.ToSourceDetails();
                            dataSourceSafe.Add(eiaDataSource);
                            break;
                        default:
                            throw new NotImplementedException();

                    }
                }
                return Ok(dataSourceSafe);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok(e.ToString());
            }
        }

        [HttpPost]
        public async Task<DataSourceBase> PostDataSource(DataSourceBase source)
        {
            return await _repo.SaveDataSource(source);
        }
    }
}