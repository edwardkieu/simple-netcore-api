using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.ViewModels.Account;
using WebApi.Wrappers;

namespace WebApi.Services.Interfaces
{
    public interface IAccountService
    {
        Task<Response<LoginResponseViewModel>> AuthenticateAsync(LoginRequestViewModel request);

        Task<Response<AppUserViewModel>> RegisterAsync(RegisterViewModel request, string origin);

        Task<Response<string>> ChangePasswordAsync(ChangePasswordViewModel model);

        Task<string> GetUserName(string userId);

        Task<Response<AppUserViewModel>> GetUserAsync(string userId);

        Task<PagedResponse<IEnumerable<AppUserViewModel>>> GetAllUsersAsync(GetAllRequestViewModel vm);
    }
}