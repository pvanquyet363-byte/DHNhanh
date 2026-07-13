using Deobely_sync.Model;
using Deobely_sync.Service.Interface;
using Infrastructure.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Deobely_sync.Service.Impliment.APILog;

namespace Deobely_sync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthentication _auth;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthentication auth, ILogger<AuthenticationController> logger)
        {
            _auth = auth;
            _logger = logger;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.UserName == "admin" && model.Password == "Abc@123456")
                    {
                        return Ok(await _auth.Authenticate(model));
                    }
                    return BadRequest("Wrong Username or Password!");
                }
                else
                {
                   
                    return BadRequest("Invalid Payload!");
                }
            }
            catch(Exception ex)
            {
                ApiLog.logval("Login", ex.Message);
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> refreshToken([FromBody] UserRefreshTokenModel model)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Refresh Failed");
                }
                else
                {
                    var response = await _auth.GetRefreshTokenAsync(model);
                    if (response != null)
                    {
                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest("Refresh Failed");
                    }
                }
            }
           
              catch (Exception ex)
            {
                ApiLog.logval("refreshToken", ex.Message);
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }


        [AllowAnonymous]
        [HttpGet("/Log")]
        public async Task<ActionResult> GetLog()
        {
            try
            {
                var res = await _auth.GetLog();
                var stream = new MemoryStream();
                int length = 100;
                if (res != null)
                {
                    stream = ExcelHelper.ExportExcel<ErrorLog>(res, ref length, 1000, null);
                    //ExcelHelper.ExportExcel<EduListNXBExportModel>(res.ListModels,"c:\\tmp\\test.xls", ref totalRow, 1000);

                }
                //return File(stream, "application/octet-stream", "danh_sach_TK_phong.xlsx");
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "danh_sach_TK_phong.xlsx");
            }

            catch (Exception ex)
            {
                ApiLog.logval("refreshToken", ex.Message);
                _logger.LogError(ex.Message);
                return BadRequest(ex);
            }
        }


    }
}
