using Deobely_sync.Model;

namespace Deobely_sync.Service.Interface
{
    public interface IAuthentication
    {
        Task<TokenResponse> Authenticate(UserModel request);
        Task<TokenResponse> GetRefreshTokenAsync(UserRefreshTokenModel model);
        Task<List<ErrorLog>> GetLog();
    }
}
