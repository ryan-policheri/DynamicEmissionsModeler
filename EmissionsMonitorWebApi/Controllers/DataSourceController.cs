using EmissionsMonitorModel.DataSources;
using EmissionsMonitorServices.Database.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmissionsMonitorWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataSourceController
    {
        private readonly DataSourceRepository _repo;
        private readonly ILogger<DataSourceController> _logger;

        public DataSourceController(DataSourceRepository repo, ILogger<DataSourceController> logger)
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
            return await _repo.UpsertDataSource(source);
        }
    }
}
