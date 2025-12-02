using DetkiProd.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DetkiProd.Controllers;

[ApiController]
[Route("api/users")]
[SwaggerTag("Контроллер для управления пользователем")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("getusers")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Получить всех пользователей")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsersAsync();
        return Ok(users);
    }

    [HttpDelete("deleteuser/{userId:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [SwaggerOperation(Summary = "Удалить пользователя")]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        await _userService.DeleteUserAsync(userId);
        return Ok();
    }
}