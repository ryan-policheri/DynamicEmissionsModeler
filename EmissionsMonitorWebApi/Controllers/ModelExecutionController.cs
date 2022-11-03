using DotNetCommon.Extensions;
using EmissionsMonitorDataAccess;
using EmissionsMonitorDataAccess.Abstractions;
using EmissionsMonitorModel.DataSources;
using EmissionsMonitorModel.ProcessModeling;
using Microsoft.AspNetCore.Mvc;

namespace EmissionsMonitorWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelExecutionController : ControllerBase
    {
        private readonly IVirtualFileSystemRepository _repo;
        private readonly ModelInitializationService _modelInitService;
        private readonly ModelExecutionService _executionService;
        private readonly ILogger<ModelExecutionController> _logger;

        public ModelExecutionController(IVirtualFileSystemRepository repo, ModelInitializationService initService, ModelExecutionService executionService, ILogger<ModelExecutionController> logger)
        {
            _repo = repo;
            _modelInitService = initService;
            _executionService = executionService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ModelExecutionResult> PostDataSource(ModelExecutionSpec spec)
        {
            spec.Model = (await _repo.GetModelSaveItemAsync(spec.ModelId)).ToProcessModel();
            await _modelInitService.InitializeModel(spec.Model);
            var stuff = await _executionService.ExecuteModelAsync(spec);
            return new ModelExecutionResult
            {
                Stuff = stuff
            };
        }
    }
}
