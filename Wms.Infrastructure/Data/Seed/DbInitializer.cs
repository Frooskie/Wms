using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Seed;

public static class DbInitializer
{
    private const string ChiefEmail = "chief@wms.com";
    private static readonly List<string> RoleNames = ["Chief", "Manager", "Worker", "StoreDirector"];
    
    public static async Task InitializeAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        foreach (var roleName in RoleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        
        var chiefUser = await userManager.FindByEmailAsync(ChiefEmail);
        
        if (chiefUser == null)
        {
            var chief = new ApplicationUser
            {
                UserName = ChiefEmail,
                Email = ChiefEmail,
                FullName = "Chief Administrator",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(chief, "Chief@123");
            
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(chief, "Chief");
            }
            else
            {
                throw new Exception("Failed to create Chief user");
            }
        }
    }
}