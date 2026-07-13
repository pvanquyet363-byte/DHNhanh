using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Deobely_sync.Service.Impliment;

namespace Auto_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        private IConfiguration _config;
        private ICustomer _repo;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(IConfiguration config, ICustomer repo)
        {
            _config = config;
            _repo = repo;
        }

        [AllowAnonymous]
        [HttpPost("AddNew")]
        public async Task<IActionResult> CreateCustomer(CustomerModel model)
        {

            JObject body_temp2 = JObject.FromObject(model);
            APILog.log($"Add New Customer", Request.Headers, body_temp2, "send ok", "");
            int a = await _repo.CreateCustomer(model);
            if(a == 1)
            {
                return Ok($"Customer {model.ma_kh} created");
            }
            if(a==2) return BadRequest("ma_kh existed");
            return BadRequest();
        } 


        }
}