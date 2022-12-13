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
                return Ok(await _repo.GetAllDataSourcesAsync());
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