using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Deobely_sync.Service.Impliment.APILog;

namespace Deobely_sync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private IConfiguration _config;
        private ICreateOrder _repo;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IConfiguration config, ICreateOrder repo, ILogger<OrderController> logger)
        {
            _config = config;
            _repo = repo;
            _logger = logger;   
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(OrderModel model)
        {
            try {
                var item = new Item();
                if (!ModelState.IsValid) return BadRequest(ModelState);
                if (model.Items[0] == item) return BadRequest("Items should not be blank");
                var result = await _repo.CreateOrder(model);
                if (result == null) return BadRequest("No data Found!");
                return Ok("Order Created");
            }
            catch(Exception e)
            {
                ApiLog.logval("Create Order", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);   
            }

        }
    }
}
