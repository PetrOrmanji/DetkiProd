using Telegram.Bot.Types;

namespace DetkiProd.Infrastructure.Telegram;

public interface ITelegramUpdateHandler
{
    Task HandleAsync(Update update, CancellationToken cancellationToken);
}

public class TelegramUpdateHandler : ITelegramUpdateHandler
{
    public Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

