using Auto_Api.Service.Impliment;
using Auto_Api.Service.Interface;
using Deobely_sync.Model;
using Deobely_sync.Service.Impliment;
using Deobely_sync.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using static Deobely_sync.Service.Impliment.APILog;

namespace Deobely_sync.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TbcController : Controller
    {
        private IConfiguration _config;
        private ITbc _repo;
        private IProduct _product;
        private readonly ILogger<TbcController> _logger;
        private ICustomer _customer;
        private ITranslateProvider _trans;
        public TbcController(IConfiguration config, IProduct product, ICustomer customer, ITranslateProvider trans, ITbc repo)
        {
            _trans= trans;
            _product= product;
            _customer = customer;
            _config = config;
            _repo = repo;
        }

        [AllowAnonymous]
        [HttpPost("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice(TbcModel model)
        {
            try
            {
                JObject body_temp2 = JObject.FromObject(model);
                APILog.log($"CreateInvoice", Request.Headers, body_temp2, "send ok", "");
                var result = await _repo.CreateTbc( model);
                if (result == null) return BadRequest("No data Found!");
                return Ok(result);
                ApiLog.logval("GetProduct", result);
            }
            catch (Exception e)
            {
                ApiLog.logval("GetProduct", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }





        [HttpPost("PhieuNhap")]
        public async Task<IActionResult> CreateInvoice2(TbcModel model)
        {
            try
            {
                JObject body_temp2 = JObject.FromObject(model);
                APILog.log($"PhieuNhap", Request.Headers, body_temp2, "send ok", "");
                var result = await _repo.CreateTbc2(model);
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

        public static bool IsDoubleRealNumber(string valueToTest)
        {
            if (double.TryParse(valueToTest, out double d) && !Double.IsNaN(d) && !Double.IsInfinity(d))
            {
                return true;
            }

            return false;
        }

        //[HttpGet("HelloWorld")]
        //public async Task<IActionResult> Hello()
        //{
        //    var result = await _trans.ExecuteAsync("吗", "vi");

        //    return Ok(result);
        //}

        [HttpPost("PhieuNhapV2")]
        public async Task<IActionResult> CreateInvoice3(Object model)
        {
            try
            {
                
                string data_temp = model.ToString();
                JObject json = JObject.Parse(data_temp);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string a = "insert into #m(";
                string b = "";
                string c = "";
                string f = "";
                foreach (JProperty property in json.Properties())
                {
                    if (property.Name.ToString() != "detail") b += property.Name + ",";
                }
                b = b.Remove(b.Length - 1, 1);
                b += ")values(";
                
                foreach (JProperty property in json.Properties())
                {
                    if (property.Name.ToString() != "detail")
                    {
                        if(IsDoubleRealNumber(property.Value.ToString()))
                        {
                            c += property.Value + ",";
                        }
                        else
                        {

                            c += "'" + property.Value + "'" + ",";
                        }
                    }    
                }

                foreach (JProperty property in json.Properties())
                {
                    if (property.Name.ToString() == "detail")
                    {

                        JArray listitem = (JArray)json["detail"];
                        for (int i = 0; i < listitem.Count; i++)
                        {
                            var item = (JObject)listitem[i];
                            string d = "insert into #d(";
                            string e = ")values(";
                            foreach (JProperty items in item.Properties())
                            {
                               

                               
                                d += items.Name + ",";
                                 if (IsDoubleRealNumber(items.Value.ToString()))
                                {
                                    e += items.Value + ",";
                                }
                                else
                                {

                                    c += "'" + items.Value + "'" + ",";
                                }
                            }

                            d = d.Remove(d.Length - 1, 1);
                            e = e.Remove(e.Length - 1, 1);
                            e += ")";
                            f += Environment.NewLine + d + e;

                        }
                    }
                }

                c = c.Remove(c.Length - 1, 1);
                c += ")";


                return Ok(a+b+c+Environment.NewLine+f);
            }

            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



    }
}
