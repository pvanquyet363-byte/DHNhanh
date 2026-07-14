using API_Adaption.Model;
using Deobely_sync.DAL;
using Deobely_sync.Infrastructure;
using Deobely_sync.Service.Impliment;
using Deobely_sync.Service.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace Deobely_sync.HostedServices
{
    /// <summary>
    /// Background consumer chạy tuần tự: đọc queue -> check log_webhook_nhanh ->
    /// nếu chưa sync thì gọi Nhanh API qua rate limiter -> lưu ERP.
    /// Chạy 1 reader duy nhất nên không bao giờ vượt 50 req/100s.
    /// </summary>
    public class NhanhWebhookProcessor : BackgroundService
    {
        private readonly NhanhWebhookQueue _queue;
        private readonly IServiceProvider _sp;
        private readonly ILogger<NhanhWebhookProcessor> _logger;

        public NhanhWebhookProcessor(
            NhanhWebhookQueue queue,
            IServiceProvider sp,
            ILogger<NhanhWebhookProcessor> logger)
        {
            _queue = queue;
            _sp = sp;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var item in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await ProcessOne(item);
                }
                catch (Exception ex)
                {
                    APILog.ApiLog.logval("err_processor_item",
                        $"orderId={item.OrderId}, ex={ex.Message}");
                }
            }
        }

        private async Task ProcessOne(WebhookItem item)
        {
            // 1. Check đã sync chưa
            string sql = $"select top 1 ngay_db from log_webhook_nhanh " +
                         $"where rtrim(ltrim(OrderId))='{item.OrderId.Trim()}'";
            DataSet ds = DAL_SQL.ExecuteGetlistDataset(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                APILog.ApiLog.logval("webhook_skip_synced", item.OrderId);
                return;
            }

            using var scope = _sp.CreateScope();
            var hook = scope.ServiceProvider.GetRequiredService<IWebHook>();

            if (item.Kind == WebhookEventKind.OrderAdd)
            {
                var order = JsonConvert.DeserializeObject<OrderNhanhResModel>(item.RawPayload);
                if (order == null) return;
                if (!string.Equals(order.Status, "Confirmed", StringComparison.Ordinal))
                {
                    APILog.ApiLog.logval("webhook_skip_not_confirmed_add",
                        $"orderId={item.OrderId}, status={order.Status}");
                    return;
                }
                int result = await hook.AddOrder(order);
                APILog.ApiLog.logval("webhook_add_result",
                    $"orderId={item.OrderId}, result={result}");
            }
            else
            {
                // orderUpdate: webhook payload không có chi tiết vận đơn,
                // BẮT BUỘC phải gọi Nhanh API -> đi qua rate limiter
                var detail = await NhanhRateLimiter.RunAsync(() =>
                    hook.GetOrderDetailAsync(item.OrderId));

                if (detail == null)
                {
                    APILog.ApiLog.logval("err_webhook_detail_null", item.OrderId);
                    return;
                }

                if (!string.Equals(detail.statusCode, "Confirmed", StringComparison.Ordinal))
                {
                    APILog.ApiLog.logval("webhook_skip_not_confirmed",
                        $"orderId={item.OrderId}, statusCode={detail.statusCode}");
                    return;
                }

                int result = await hook.AddOrderUpdate(detail);
                APILog.ApiLog.logval("webhook_update_result",
                    $"orderId={item.OrderId}, result={result}");
            }
        }
    }
}
