using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface ITbc
    {
        Task<string> CreateTbc(TbcModel model);
        Task<string> CreateTbc2(TbcModel model);
    }
}
