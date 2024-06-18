using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBMF.Core;

public static class TelegramBotClientExtensions
{
    public static async Task ReplyToMessageAsync(this ITelegramBotClient botClient, Message message, string text, bool isQuote = true, ParseMode parseMode = ParseMode.Markdown)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, text, parseMode: parseMode, replyToMessageId: isQuote ? message.MessageId : null);
    }
}