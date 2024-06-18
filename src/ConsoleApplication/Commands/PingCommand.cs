using TBMF.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApplication.Commands;

[Access(000, 111)]
[TgDescription("Receive Pong!")]
public class PingCommand : ITelegramCommand
{
    public async Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update)
    {
        await botClient.ReplyToMessageAsync(update.Message, "*Pong!*");
    }
}