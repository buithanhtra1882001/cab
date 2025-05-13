using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Constants;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Startup.DataSeeder
{
    public static class DataInitializer
    {
        public static void SeedData(RoleManager<IdentityRole<int>> roleManager)
        {
            SeedRoles(roleManager).Wait();
            SeedRoleClaims(roleManager).Wait();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            if (!roleManager.RoleExistsAsync(RoleConstant.Admin).Result)
            {
                await roleManager.CreateAsync(new IdentityRole<int>(RoleConstant.Admin));
            }

            if (!roleManager.RoleExistsAsync(RoleConstant.Mod).Result)
            {
                await roleManager.CreateAsync(new IdentityRole<int>(RoleConstant.Mod));
            }

            if (!roleManager.RoleExistsAsync(RoleConstant.Staff).Result)
            {
                await roleManager.CreateAsync(new IdentityRole<int>(RoleConstant.Staff));
            }

            if (!roleManager.RoleExistsAsync(RoleConstant.User).Result)
            {
                await roleManager.CreateAsync(new IdentityRole<int>(RoleConstant.User));
            }
        }

        private static async Task SeedRoleClaims(RoleManager<IdentityRole<int>> roleManager)
        {
            var modRolePermissions = new List<string>
            {
                PermissionConstant.BanUser,
                PermissionConstant.ViewPublicUserInformation
            };
            var modRole = await roleManager.FindByNameAsync(RoleConstant.Mod);
            var currentModRoleClaims = (await roleManager.GetClaimsAsync(modRole)).Select(item => item.Value);
            foreach (var permission in modRolePermissions)
            {
                if (!currentModRoleClaims.Contains(permission))
                {
                    await roleManager.AddClaimAsync(modRole, new Claim(ClaimTypeConstant.Permission, permission));
                }
            }
        }


    }
}
