using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DetkiProd.Controllers;

[ApiController]
[Route("api/auth")]
[SwaggerTag("Контроллер для аутентификации и регистрации пользователей")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IAdminOptionsProvider _adminOptionsProvider;

    public AuthController(IAuthService authService, IAdminOptionsProvider adminOptionsProvider)
    {
        _authService = authService ?? throw new ArgumentException("Auth Service cannot be null", nameof(authService));
        _adminOptionsProvider = adminOptionsProvider ?? throw new ArgumentException("Admin Options Provider cannot be null", nameof(adminOptionsProvider));
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Вход пользователя в систему")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var token = await _authService.LoginAsync(request.Login, request.Password);
        return Ok(token);
    }

    [HttpPost("registeradmin")]
    [SwaggerOperation(Summary = "Регистрация нового администратора")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminRequest request)
    {
        if (request.AdminPassword != _adminOptionsProvider.GetAdminPassword())
        {
            return Unauthorized();
        }

        var token = await _authService.RegisterAsAdminAsync(
            request.Login,
            request.Password
        );
        return Ok(token);
    }

    [HttpPost("registeradminfromtelegram")]
    [SwaggerOperation(Summary = "Регистрация нового администратора (телеграм аккаунт)")]
    public async Task<IActionResult> RegisterAdminFromTelegram([FromBody] RegisterAdminFromTelegramRequest request)
    {
        if (request.AdminPassword != _adminOptionsProvider.GetAdminPassword())
        {
            return Unauthorized();
        }

        var token = await _authService.RegisterTelegramUserAsAdminAsync(request.TelegramUserId);
        return Ok(token);
    }
}

[SwaggerSchema("Запрос для входа в систему")]
public record LoginRequest(string Login, string Password);

[SwaggerSchema("Запрос для регистрации нового администратора")]
public record RegisterAdminRequest(string Login, string Password, string AdminPassword);

[SwaggerSchema("Запрос для регистрации нового администратора (телеграм аккаунт)")]
public record RegisterAdminFromTelegramRequest(long TelegramUserId, string AdminPassword);