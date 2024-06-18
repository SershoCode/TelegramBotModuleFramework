using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBMF.Core;

internal interface ITelegramSystemCommand
{
    Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update);
}