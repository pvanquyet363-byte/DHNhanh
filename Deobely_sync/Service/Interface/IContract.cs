using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface IContract
    {
        Task<int> CreateContract(ContractModel model);
    }
}
