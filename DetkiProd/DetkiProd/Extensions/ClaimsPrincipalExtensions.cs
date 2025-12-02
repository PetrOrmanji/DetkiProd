using System.Security.Claims;

namespace DetkiProd.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var parsed = Guid.TryParse(userIdClaim, out var userId);
        return userId;
    }
}
