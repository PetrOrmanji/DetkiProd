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
        new() { Command = "start", Description = "🚀 Старт" },
        new() { Command = "get", Description = "🏷️ Мои заметки" },
        new() { Command = "add", Description = "➕ Добавить заметку" },
        new() { Command = "remove", Description = "🧹 Удалить заметку" },
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
