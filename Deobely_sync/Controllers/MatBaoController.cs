using API_Adaption.Model;
using Deobely_sync.Model;
using Deobely_sync.Service.Impliment;
using Deobely_sync.Service.Interface;
using DocumentFormat.OpenXml.Vml;
using Infrastructure.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace API_Adaption.Controllers
{
    public class MatBaoController : Controller
    {
        private IConfiguration _config;
        private readonly ILogger<MatBaoController> _logger;
        private IMatBao _matbao;
        public MatBaoController(IConfiguration config, IMatBao matbao)
        {
            _config = config;
            _matbao = matbao;
        }

        [AllowAnonymous]
        [HttpPost("MatBao/GetInvoice")]
        public async Task<IActionResult> GetInvoice([FromBody] ReqParamModel param)
        {
            try
            {
                ResponseModel res = await _matbao.GetInvoice(param);
                return Ok(res);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("er_webhooknhanh_test", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet("/")]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            return Ok("Ok");
        }
        [HttpGet("LINH_TEST")]
        [AllowAnonymous]
        public async Task<IActionResult> LINH_TEST()
        {
          var version = "LINH_TEST 1.6";
          return Ok(version);
        }
        [HttpGet("CA_TEST")]
        [AllowAnonymous]
        public async Task<IActionResult> CA_TEST()
        {
            var version = "CA_TEST 0.1";
            return Ok(version);
        }
    }
}
