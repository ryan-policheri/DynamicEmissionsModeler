using EmissionsMonitorDataAccess.Database;
using Microsoft.AspNetCore.Mvc;

namespace EmissionsMonitorWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {

        private readonly EmissionsMonitorContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(EmissionsMonitorContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //[HttpPost(Name = "InitializeDatabase")]
        //public ActionResult Get()
        //{
        //    DbInitializer.Initialize(_context);
        //    return Ok();
        //}

        [HttpGet("ping")]
        public string Ping()
        {
            return "Connection to api successful!";
        }
    }
}