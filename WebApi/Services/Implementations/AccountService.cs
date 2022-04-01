using Application.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApi.Data.Entities;
using WebApi.Exceptions;
using WebApi.Services.Interfaces;
using WebApi.Settings;
using WebApi.ViewModels.Account;
using WebApi.ViewModels.Email;
using WebApi.Wrappers;

namespace WebApi.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        
        public AccountService(UserManager<AppUser> userManager,
            IOptions<JWTSettings> jwtSettings,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
        }

        public async Task<Response<LoginResponseViewModel>> AuthenticateAsync(LoginRequestViewModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new ApiException($"No Accounts Registered with {request.Email}.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new ApiException($"Invalid Credentials for '{request.Email}'.");
            }

            if (!user.EmailConfirmed)
            {
                throw new ApiException($"Account Not Confirmed for '{request.Email}'.");
            }

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            var jwtSecurityToken = await GenerateJWToken(user);
            var response = new LoginResponseViewModel
            {
                Id = user.Id,
                JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName,
                Roles = rolesList.ToList(),
                IsVerified = user.EmailConfirmed
            };

            return new Response<LoginResponseViewModel>(response, $"Authenticated {user.UserName}");
        }

        public async Task<Response<AppUserViewModel>> RegisterAsync(RegisterViewModel request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already taken.");
            }

            var user = new AppUser
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                EmailConfirmed = true
            };

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.User.ToString());
                    AppUserViewModel userVM = new()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Roles = new List<string> { Roles.User.ToString() },
                        IsVerified = user.EmailConfirmed,
                    };
                    return new Response<AppUserViewModel>(userVM, message: $"User Registered.");
                }

                throw new ApiException("Passwords must have at least one uppercase ('A'-'Z').");
            }

            throw new ApiException($"Email {request.Email } is already registered.");
        }

        private async Task<JwtSecurityToken> GenerateJWToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }

        public async Task<Response<string>> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);

            if (account == null) 
                throw new ApiException($"No Accounts Registered with {model.Email}.");

            var result = await _userManager.ChangePasswordAsync(account, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return new Response<string>(model.Email, message: $"Password changed.");
            }
            else
            {
                throw new ApiException($"Error occured while change the password.");
            }
        }

        public async Task<string> GetUserName(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return string.Empty;
            return user.UserName;
        }

        public async Task<Response<AppUserViewModel>> GetUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApiException($"User {userId} not found.");
            }

            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            AppUserViewModel userContext = new()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                Roles = rolesList.ToList(),
                IsVerified = user.EmailConfirmed,
            };

            return new Response<AppUserViewModel>(userContext);
        }

        public async Task<PagedResponse<IEnumerable<AppUserViewModel>>> GetAllUsersAsync(GetAllRequestViewModel vm)
        {
            var query = _userManager.Users;
            var totalCount = await query.CountAsync();

            var users = await query.Skip((vm.PageNumber - 1) * vm.PageSize).Take(vm.PageSize).Select(x => new AppUserViewModel
            {
                Id = x.Id,
                Email = x.Email,
                UserName = x.UserName,
                FullName = x.FullName,
                Roles = new List<string>(),
                IsVerified = x.EmailConfirmed,
            }).ToListAsync();

            return new PagedResponse<IEnumerable<AppUserViewModel>>(users, totalCount, vm.PageNumber, vm.PageSize);
        }
    }
}