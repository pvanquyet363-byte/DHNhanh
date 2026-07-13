using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface ICustomer
    {
        Task<int> CreateCustomer(CustomerModel model);
    }
}
