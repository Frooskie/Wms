using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Auth;
using Wms.API.Extensions;
using Wms.Core.Entities;
using Wms.Core.Interfaces.Services.Auth;

namespace Wms.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IJwtService jwtService,
    IValidator<LoginRequest> loginValidator,
    IValidator<RegisterRequest> registerValidator)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);
        
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized("Invalid credentials");

        var roles = await userManager.GetRolesAsync(user);
        var token = jwtService.GenerateToken(user, roles);

        return Ok(new AuthResponse
        {
            Token = token,
            Email = user.Email,
            FullName = user.FullName,
            Roles = roles.ToList()
        });
    }

    [Authorize(Roles = "Chief")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);
        
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        if (!string.IsNullOrEmpty(request.Role))
        {
            var roleExists = await roleManager.RoleExistsAsync(request.Role);
            if (!roleExists)
                return BadRequest($"Role '{request.Role}' does not exist");
            await userManager.AddToRoleAsync(user, request.Role);
        }

        return Ok(new { Message = "User registered successfully" });
    }
}