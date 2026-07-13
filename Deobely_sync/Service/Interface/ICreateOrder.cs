using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface ICreateOrder
    {
        Task<string> CreateOrder(OrderModel ma_vt2);
    }
}
