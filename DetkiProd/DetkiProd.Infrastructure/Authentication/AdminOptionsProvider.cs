using DetkiProd.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace DetkiProd.Infrastructure.Authentication;

public class AdminOptionsProvider : IAdminOptionsProvider
{
    private readonly AdminOptions _options;

    public AdminOptionsProvider(IOptions<AdminOptions> options)
    {
        _options = options.Value;
    }

    public string GetAdminPassword()
    {
        return _options.Password;
    }
}
