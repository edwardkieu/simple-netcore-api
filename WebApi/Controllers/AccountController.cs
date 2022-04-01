using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Services.Interfaces;
using WebApi.ViewModels.Account;
using WebApi.Wrappers;

namespace WebApi.Controllers
{
    public class AccountController : ApiControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public AccountController(IAccountService accountService, IAuthenticatedUserService authenticatedUserService)
        {
            _accountService = accountService;
            _authenticatedUserService = authenticatedUserService;
        }

        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfoAsync()
        {
            var result = new Response<AppUserViewModel>(_authenticatedUserService.CurrentUser);
            return Ok(await Task.FromResult(result));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> AuthenticateAsync(LoginRequestViewModel request)
        {
            return Ok(await _accountService.AuthenticateAsync(request));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _accountService.RegisterAsync(request, origin));
        }

        [HttpPost("change-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            return Ok(await _accountService.ChangePasswordAsync(model));
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllUsersAsync(GetAllRequestViewModel vm)
        {
            var users = await _accountService.GetAllUsersAsync(vm);

            return Ok(users);
        }
    }
}