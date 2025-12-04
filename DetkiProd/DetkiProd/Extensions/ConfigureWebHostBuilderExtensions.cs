namespace DetkiProd.Extensions;

public static class ConfigureWebHostBuilderExtensions
{
    public static IWebHostBuilder UseUrlsIfExist(this ConfigureWebHostBuilder builder, IConfiguration configuration)
    {
        var urls = configuration["Urls"];

        if (string.IsNullOrWhiteSpace(urls))
        {
            return builder;
        }
        
        return builder.UseUrls(urls);
    }
}
