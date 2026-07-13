using System.Threading.Channels;

namespace Deobely_sync.Infrastructure
{
    /// <summary>
    /// Hàng đợi xử lý webhook Nhanh. Action Webhook chỉ đẩy vào queue rồi Ok(1) ngay,
    /// tránh vượt rate limit 50/100s của Nhanh khi nhận nhiều webhook cùng lúc.
    /// </summary>
    public sealed class NhanhWebhookQueue
    {
        private readonly Channel<WebhookItem> _channel = Channel.CreateUnbounded<WebhookItem>(
            new UnboundedChannelOptions { SingleReader = true });

        public ChannelWriter<WebhookItem> Writer => _channel.Writer;
        public ChannelReader<WebhookItem> Reader => _channel.Reader;
    }

    public enum WebhookEventKind { OrderAdd, OrderUpdate }

    public class WebhookItem
    {
        public WebhookEventKind Kind { get; set; }
        public string OrderId { get; set; } = "";
        public string RawPayload { get; set; } = "";
    }
}
