﻿using Application.Enums;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.Entities;

namespace WebApi.Data.Seeds
{
    public static class DefaultNormalUser
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default User
            var defaultUser = new AppUser
            {
                UserName = "local",
                Email = "local@gmail.com",
                FullName = "Normal User",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Abcd1234");
                    await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
                }
            }
        }
    }
}