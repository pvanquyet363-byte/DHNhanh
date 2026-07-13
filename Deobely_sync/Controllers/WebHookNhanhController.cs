using API_Adaption.Model;
using Deobely_sync;
using Deobely_sync.DAL;
using Deobely_sync.Infrastructure;
using Deobely_sync.Model;
using Deobely_sync.Service.Impliment;
using Deobely_sync.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace API_Adaption.Controllers
{
    public class WebHookNhanhController : Controller
    {
        private IConfiguration _config;
        private readonly ILogger<WebHookNhanhController> _logger;
        private IWebHook _webhook;
        private readonly NhanhWebhookQueue _queue;

        public WebHookNhanhController(
            IConfiguration config,
            IWebHook webhook,
            NhanhWebhookQueue queue)
        {
            _config = config;
            _webhook = webhook;
            _queue = queue;
        }

        [AllowAnonymous]
        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            try
            {
                object t = Request.QueryString;
                APILog.log("webhook_nhanh_test", Request.Headers, "", t, "");
                _webhook.Test(t);
                return Ok(1);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("er_webhooknhanh_test", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("Version")]
        public async Task<IActionResult> Version()
        {
            return Ok("1.1.1");
        }

        [AllowAnonymous]
        [HttpPost("CallBackUrl")]
        public async Task<IActionResult> PostTest([FromBody] Object val)
        {
            try
            {
                object t = Request.QueryString;
                APILog.log("webhook_nhanh_p", Request.Headers, val, t, "");
                return Ok(1);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("GetProduct", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("ResetAccesToken")]
        public async Task<IActionResult> ResetAccesToken()
        {
            try
            {
                APILog.log("webhook_nhanh_reset_token", "", Request, "send ok", "");
                return Ok(1);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("err_Reset_AccessToken", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Webhook Nhanh - trả về 1 ngay lập tức, xử lý nền qua BackgroundService.
        /// Tránh vượt rate limit 50/100s khi Nhanh bùng nổ bắn webhook.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Webhook")]
        public async Task<IActionResult> Webhook([FromBody] Object? val)
        {
            try
            {
                if (val == null)
                {
                    APILog.log("webhook_nhanh_send", "null obj", "Khong co obj", "send error", "");
                    return Ok(1);
                }

                APILog.log("webhook_nhanh_send", Request.Headers, val, "send ok", "");
                string verifyToken = _config.GetValue<string>("NhanhVn:webhooksVerifyToken");
                string idNhanh = _config.GetValue<string>("NhanhVn:idNhanh");

                string data_temp = val.ToString();
                JObject obj = JObject.Parse(data_temp);

                string resEvent = (string)obj["event"] ?? "";
                if (resEvent != "orderUpdate" && resEvent != "orderAdd") return Ok(1);

                string resId = (string)obj["businessId"] ?? "";
                string resToken = (string)obj["webhooksVerifyToken"] ?? "";

                if (resId != idNhanh || resToken != verifyToken) return Ok(1);

                OrderNhanhResModel data = JsonConvert.DeserializeObject<OrderNhanhResModel>(
                    obj["data"].ToString());
                string orderId = data.OrderId.ToString();
                string rawPayload = obj["data"].ToString();

                // Đẩy vào queue - BackgroundService sẽ xử lý tuần tự
                var kind = resEvent == "orderAdd"
                    ? WebhookEventKind.OrderAdd
                    : WebhookEventKind.OrderUpdate;

                _queue.Writer.TryWrite(new WebhookItem
                {
                    Kind = kind,
                    OrderId = orderId,
                    RawPayload = rawPayload
                });

                APILog.ApiLog.logval("webhook_enqueued",
                    $"event={resEvent}, orderId={orderId}");

                return Ok(1);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("err_WebHook_send", e.Message);
                return Ok(1);
            }
        }

        /// <summary>
        /// Batch đồng bộ - chỉ đẩy OrderId vào queue, BackgroundService sẽ lo phần còn lại.
        /// 24 đơn sẽ được xử lý tuần tự, mỗi đơn cách nhau ~2.5s -> tổng ~60s.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("Webhook/abc")]
        public async Task<IActionResult> ResetToken2()
        {
            try
            {
                string sql = $"select * from log_webhook_nhanh where OrderId in ('483651424','483650488','483644818','483639364','483638759','483635382','483604685','483604420','483603764','483606054','483602224','483592019','483591821','483588493','483573088','483512418','483507239','483467781','483467027','483430193','483404088','483388187','483343634','483082593')";
                DataSet ds = DAL_SQL.ExecuteGetlistDataset(sql);

                int count = 0;
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    string orderId = item["OrderId"].ToString();
                    _queue.Writer.TryWrite(new WebhookItem
                    {
                        Kind = WebhookEventKind.OrderUpdate,
                        OrderId = orderId,
                        RawPayload = ""
                    });
                    count++;
                }

                APILog.ApiLog.logval("webhook_batch_enqueued", $"count={count}");
                return Ok($"Enqueued {count} orders");
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("err_WebHook_send", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Webhook/ResetToken")]
        public async Task<IActionResult> ResetToken()
        {
            try
            {
                string re = await _webhook.ResetAcessToken();
                return Ok(re);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("err_WebHook_send", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Webhook/GetCity")]
        public async Task<IActionResult> GetCity()
        {
            try
            {
                _webhook.GetCity();
                return Ok(1);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("err_WebHook_send", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Webhook/GetDistrict")]
        public async Task<IActionResult> GetDistrict()
        {
            try
            {
                string re = await _webhook.GetDistrict();
                return Ok(1);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("err_WebHook_send", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Webhook/GetWard")]
        public async Task<IActionResult> GetWard()
        {
            try
            {
                string re = await _webhook.GetWard();
                return Ok(1);
            }
            catch (Exception e)
            {
                APILog.ApiLog.logval("err_WebHook_send", e.Message);
                _logger.LogError(e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}
