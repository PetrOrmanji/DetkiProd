using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DetkiProd.Infrastructure.Telegram;

public class TelegramBotHostedService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramBotHostedService> _logger;

    private readonly BotCommand[] BotCommands =
    {
        new() { Command = "start",          Description = "🚀 Старт" },

        new() { Command = "getfiles",       Description = "📂 Файлы" },
        new() { Command = "uploadfile",     Description = "⬆️ Отправить файл" },
        new() { Command = "downloadfile",   Description = "⬇️ Загрузить файл" },
        new() { Command = "deletefile",     Description = "🗑️ Удалить файл" },
        new() { Command = "getfileurl",     Description = "🔗 Получить ссылку" },

        new() { Command = "getprojects",    Description = "📁 Проекты" },
        new() { Command = "addproject",     Description = "➕ Добавить проект" },
        new() { Command = "deleteproject",  Description = "❌ Удалить проект" }
    };

    public TelegramBotHostedService(
        ITelegramBotClient botClient,
        IServiceProvider serviceProvider,
        ILogger<TelegramBotHostedService> logger)
    {
        _botClient = botClient;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _botClient.SetMyCommands(BotCommands, cancellationToken: stoppingToken);

        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            cancellationToken: stoppingToken
        );

        _logger.LogInformation("Telegram bot started");
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();

        try
        {
            var handler = scope.ServiceProvider
                .GetRequiredService<ITelegramUpdateHandler>();

            await handler.HandleAsync(update, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling update");

            try
            {
                if (update.Message?.Chat?.Id is { } chatId)
                {
                    await _botClient.SendMessage(
                        chatId: chatId,
                        text: $"🔴 Непредвиденная ошибка, используй /start.",
                        cancellationToken: cancellationToken);
                }
            }
            catch { }
        }
    }

    private Task HandleErrorAsync(
        ITelegramBotClient botClient,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram bot error");
        return Task.CompletedTask;
    }
}
