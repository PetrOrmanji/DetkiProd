using Telegram.Bot;

namespace DetkiProd.Infrastructure.Telegram;

public class TelegramBotClientExtended : TelegramBotClient
{
    public string BotApiLocalServerFilesPath { get; }

    public TelegramBotClientExtended(
        string botApiLocalServerFilesPath,
        TelegramBotClientOptions options, 
        HttpClient? httpClient,
        CancellationToken cancellationToken = default) 
        : base(options, httpClient, cancellationToken)
    {
        BotApiLocalServerFilesPath = botApiLocalServerFilesPath
           ?? throw new ArgumentNullException(nameof(botApiLocalServerFilesPath));
    }

    public async Task GetFileFromLocalServer(string filePath, Stream destination)
    {
        var filePathSplitted = filePath.Split("/");

        var token = filePathSplitted[^3];
        var folder = filePathSplitted[^2];
        var fileName = filePathSplitted[^1];

        var botApiLocalServerFilesPath = Path.Combine(BotApiLocalServerFilesPath, token, folder, fileName);
        if(!File.Exists(botApiLocalServerFilesPath))
        {
            botApiLocalServerFilesPath = Path.Combine(BotApiLocalServerFilesPath, token.Replace(':', ''), folder, fileName);
            
            if(!File.Exists(botApiLocalServerFilesPath))
            {
                throw new FileNotFoundException(botApiLocalServerFilesPath);
            }
        }

        await using var fileStream = new FileStream(botApiLocalServerFilesPath, FileMode.Open);
        await fileStream.CopyToAsync(destination);
    }
}
