using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Security.Claims;
using WebApi.Services.Interfaces;
using WebApi.ViewModels.Account;

namespace WebApi.Services.Implementations
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor, IAccountService accountService)
        {
            _httpContextAccessor = httpContextAccessor;
            _accountService = accountService;
        }

        public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");

        public AppUserViewModel CurrentUser => _accountService.GetUserAsync(UserId).GetAwaiter().GetResult()?.Data;

        public string Username => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        public string GetSpecificClaim(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(x => x.Type == claimType);
            return (claim != null) ? claim.Value : string.Empty;
        }

        public string GetHostNameFromRemoteIpAddress()
        {
            var remoteIp = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;
            var hostEntry = Dns.GetHostEntry(remoteIp);

            return hostEntry.HostName;
        }
    }
}