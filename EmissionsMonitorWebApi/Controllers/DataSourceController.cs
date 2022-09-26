using EmissionsMonitorModel.DataSources;
using EmissionsMonitorDataAccess.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace EmissionsMonitorWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataSourceController
    {
        private readonly IDataSourceRepository _repo;
        private readonly ILogger<DataSourceController> _logger;

        public DataSourceController(IDataSourceRepository repo, ILogger<DataSourceController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<DataSourceBase>> GetAllDataSources()
        {
            return await _repo.GetAllDataSourcesAsync();
        }

        [HttpPost]
        public async Task<DataSourceBase> PostDataSource(DataSourceBase source)
        {
            return await _repo.SaveDataSource(source);
        }
    }
}