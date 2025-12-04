using DetkiProd.Extensions;
using DetkiProd.Infrastructure.Extensions;
using DetkiProd.Application.Extensions;
using System.Reflection;

SetCurrentDirectory();
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrlsIfExist(builder.Configuration);

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddSecurityServices();
builder.Services.AddApplicationServices();
builder.Services.AddTelegramBot(builder.Configuration);
builder.Services.AddCacheServices(builder.Configuration);
builder.Services.AddSwaggerServices();

builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwaggerIfDevelopment();
app.UseGlobalExceptionMiddleware();

app.MapControllers();

app.Run();

static void SetCurrentDirectory()
{
    var entryAssembly = Assembly.GetEntryAssembly();
    if (entryAssembly is null)
    {
        return;
    }

    var assemblyDirectory = Path.GetDirectoryName(entryAssembly.Location);
    if (assemblyDirectory is null)
    {
        return;
    }

    Directory.SetCurrentDirectory(assemblyDirectory);
}
