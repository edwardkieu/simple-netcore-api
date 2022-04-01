using WebApi.ViewModels.Account;

namespace WebApi.Services.Interfaces
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
        string Username { get; }
        AppUserViewModel CurrentUser { get; }

        string GetSpecificClaim(string claimType);
    }
}