using Telegram.Bot;

namespace TBMF.Core;

public interface ITelegramPeriodicEvent
{
    public string CronExpression { get; set; }

    public Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions);
}