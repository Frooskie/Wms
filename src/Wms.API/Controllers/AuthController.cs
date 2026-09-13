using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wms.API.DTOs.Auth;
using Wms.API.Extensions;
using Wms.Core.Constants;
using Wms.Core.Entities;
using Wms.Core.Exceptions;
using Wms.Core.Interfaces.Services.Auth;

namespace Wms.API.Controllers;

/// <summary>Аутентификация и регистрация пользователей.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IJwtService jwtService,
    IValidator<LoginRequest> loginValidator,
    IValidator<RegisterRequest> registerValidator)
    : ControllerBase
{
    /// <summary>Вход в систему. Возвращает JWT-токен и информацию о пользователе.</summary>
    /// <response code="200">Успешный вход. Возвращает объект AuthResponse.</response>
    /// <response code="400">Ошибка валидации запроса.</response>
    /// <response code="401">Неверные учётные данные.</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedException(ErrorMessages.Auth.InvalidCredentials);

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

    /// <summary>Регистрация нового пользователя. Доступно только для Chief.</summary>
    /// <response code="200">Пользователь зарегистрирован.</response>
    /// <response code="400">Ошибка валидации или указана несуществующая роль.</response>
    /// <response code="401">Не авторизован (отсутствует JWT-токен).</response>
    /// <response code="403">Недостаточно прав (требуется роль Chief).</response>
    [Authorize(Roles = "Chief")]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(ProblemDetails))]
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
        {
            var errors = result.Errors
                .Select(e => new ValidationError(e.Code, e.Description))
                .ToList();
            throw new ModelValidationException(ErrorMessages.Auth.UserCreationFailed, errors);
        }

        if (!string.IsNullOrEmpty(request.Role))
        {
            var roleExists = await roleManager.RoleExistsAsync(request.Role);
            if (!roleExists)
                throw new BusinessRuleException(
                    string.Format(ErrorMessages.Auth.RoleNotFoundFormat, request.Role),
                    "ROLE_NOT_FOUND");
            
            await userManager.AddToRoleAsync(user, request.Role);
        }

        return Ok(new RegisterResponse { Message = ErrorMessages.Success.UserRegistered });
    }
}