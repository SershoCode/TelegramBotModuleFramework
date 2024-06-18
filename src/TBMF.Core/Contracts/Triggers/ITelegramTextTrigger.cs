using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBMF.Core;

public interface ITelegramTextTrigger
{
    public IEnumerable<string> Triggers { get; set; }
    public TextTriggerType TextTriggerType { get; set; }
    public int TextTriggerChancePercentage { get; set; }

    Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update);
}