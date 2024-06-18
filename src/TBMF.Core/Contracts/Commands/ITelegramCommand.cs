using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBMF.Core;

public interface ITelegramCommand
{
    Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update);
}