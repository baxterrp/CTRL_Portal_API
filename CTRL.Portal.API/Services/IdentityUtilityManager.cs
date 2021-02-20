using Microsoft.AspNetCore.Identity;
using System;

namespace CTRL.Portal.API.Services
{
    public static class IdentityUtilityManager
    {
        private static readonly IdentityRole[] _roles =
        {
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString()
            },
            new IdentityRole                
            {
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString()
            },
        };

        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in _roles)
            {
                var roleExists = roleManager.RoleExistsAsync(role.Name).Result;

                if (!roleExists)
                {
                    roleManager.CreateAsync(role).Wait();
                }
            }
        }
    }
}
