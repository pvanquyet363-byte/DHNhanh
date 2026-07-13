using API_Adaption.Model;
using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface IWebHook
    {
        void Test(object t);
        Task<int> AddOrder(OrderNhanhResModel order);
        Task<int> AddOrderUpdate(NhanhVNUpdateModel order);
        Task<NhanhVNUpdateModel?> GetOrderDetailAsync(string orderId);
        Task<string> ResetAcessToken();
        void GetCity();
        Task<string> GetDistrict();
        Task<string> GetWard();
    }
}
