using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Deobely_sync.Service.Impliment.APILog;

namespace Deobely_sync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private IConfiguration _config;
        private IProduct _repo;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IConfiguration config, IProduct repo)
        {
            _config = config;
            _repo = repo;
        }
        [HttpPost("/test")]
        public async Task<IActionResult> Test()
        {
            return Ok("OK");
        }

            [HttpPost("getproduct/{ma_vt2}")]
        public async Task<IActionResult> GetProduct()
        {
            try
            {
                var result = await _repo.GetProduct();
                if (result == null) return BadRequest("No data Found!");
                return Ok(result);
            }
            catch (Exception e)
            {
                ApiLog.logval("GetProduct", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> CreateProduct(ProductInsertModel model)
        {
            try
            {
                var result = await _repo.CreateProduct(model);
                if (result == 0) return BadRequest("No data Found!");
                else if (result == 2) return BadRequest("Item Existed!");
                return Ok($"Product {model.ten_vt} Created");
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
