using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JobPortal.Models;
using System.Security.Claims;

namespace JobPortal.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            string[] roleNames = { "Admin", "HR", "Applicant" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@jobportal.com",
                Email = "admin@jobportal.com",
                EmailConfirmed = true,
                FullName = "Administrator",
                RoleType = "Admin"
            };

            if (await userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddClaimAsync(adminUser, new Claim("UserType", "Admin"));
                }
            }
            else
            {
                var existingAdmin = await userManager.FindByEmailAsync(adminUser.Email);
                var claims = await userManager.GetClaimsAsync(existingAdmin);
                if (!claims.Any(c => c.Type == "UserType" && c.Value == "Admin"))
                {
                    await userManager.AddClaimAsync(existingAdmin, new Claim("UserType", "Admin"));
                }
            }

            // Create HR user
            var hrUser = new ApplicationUser
            {
                UserName = "hr@jobportal.com",
                Email = "hr@jobportal.com",
                EmailConfirmed = true,
                FullName = "HR Manager",
                RoleType = "HR"
            };

            if (await userManager.FindByEmailAsync(hrUser.Email) == null)
            {
                var result = await userManager.CreateAsync(hrUser, "HR123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(hrUser, "HR");
                    await userManager.AddClaimAsync(hrUser, new Claim("UserType", "HR"));
                }
            }
            else
            {
                var existingHR = await userManager.FindByEmailAsync(hrUser.Email);
                var claims = await userManager.GetClaimsAsync(existingHR);
                if (!claims.Any(c => c.Type == "UserType" && c.Value == "HR"))
                {
                    await userManager.AddClaimAsync(existingHR, new Claim("UserType", "HR"));
                }
            }
        }
    }
} 