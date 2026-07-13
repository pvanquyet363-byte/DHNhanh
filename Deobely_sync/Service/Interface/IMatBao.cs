using API_Adaption.Model;
using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface IMatBao
    {
        Task<ResponseModel> GetInvoice(ReqParamModel param);

    }
}
