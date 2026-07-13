using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Deobely_sync.Service.Impliment.APILog;

namespace Deobely_sync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : Controller
    {
        private IConfiguration _config;
        private IContract _repo;
        private readonly ILogger<ContractController> _logger;
        public ContractController(IConfiguration config, IContract repo)
        {
            _config = config;
            _repo = repo;
        }

        
        [HttpPost("AddNew")]
        public async Task<IActionResult> CreateContract(ContractModel model)
        {
            try
            {
                var result = await _repo.CreateContract(model);
                if (result == 2) return BadRequest("Ma_hd Existed");
                if (result != 1) return BadRequest("No data Found!");
                return Ok(result);
            }
            catch (Exception e)
            {
                ApiLog.logval("GetProduct", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }


    }
}
