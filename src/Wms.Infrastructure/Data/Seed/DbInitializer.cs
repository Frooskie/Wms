using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wms.Core.Constants;
using Wms.Core.Entities;

namespace Wms.Infrastructure.Data.Seed;

public static class DbInitializer
{
    private const string ChiefEmail = "chief@wms.com";
    private const string ChiefPassword = "Chief@123";

    public static async Task InitializeAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        foreach (var roleName in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
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

            var result = await userManager.CreateAsync(chief, ChiefPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
                throw new InvalidOperationException($"Не удалось создать Chief-пользователя: {errors}");
            }

            await userManager.AddToRoleAsync(chief, Roles.Chief);
        }
    }
}