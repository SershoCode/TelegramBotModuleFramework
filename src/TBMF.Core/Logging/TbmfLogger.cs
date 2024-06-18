using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBMF.Core;

internal static class TbmfLogger
{
    internal static async Task LogToConsoleAsync(string message)
    {
        await Console.Out.WriteLineAsync($"[{DateTime.Now:HH:mm:ss}] [TBMF] {message}");
    }

    internal static async Task LogCommandToConsoleAsync(string commandTrigger, Update update)
    {
        var messageFrom = update?.Message?.From;
        var messageChat = update?.Message?.Chat;

        var fromUserId = messageFrom?.Id;
        var fromUserName = messageFrom?.Username;
        var fromChatId = messageChat?.Id;
        var fromChatTitle = messageChat?.Title;

        var sb = new StringBuilder();

        sb.Append("Executing /")
            .Append(commandTrigger)
            .Append(" command... | ")
            .Append("By User: ")
            .Append(fromUserId)
            .Append(fromUserName is not null ? $" ({fromUserName})" : messageFrom?.FirstName is not null ? $" ({messageFrom.FirstName} {messageFrom.LastName})" : null)
            .Append(" | ")
            .Append("In Chat: ").Append(fromChatId)
            .Append(fromChatTitle is not null ? $" ({fromChatTitle})" : null)
            .Append(fromUserId == fromChatId ? " (Private Message)" : null);

        await LogToConsoleAsync(sb.ToString());
    }

    internal static async Task LogTriggerToConsoleAsync(Type type, Update update)
    {
        var messageFrom = update?.Message?.From;
        var messageChat = update?.Message?.Chat;

        var fromUserId = messageFrom?.Id;
        var fromUserName = messageFrom?.Username;
        var fromChatId = messageChat?.Id;
        var fromChatTitle = messageChat?.Title;

        await LogToConsoleAsync($"Triggered {type.Name}... | By User: {fromUserId}{(fromUserName is not null ? $" ({fromUserName})" : null)} | In chat: {fromChatId} ({fromChatTitle})");
    }

    internal static async Task LogPeriodicalEventToConsoleAsync(Type type)
    {
        await LogToConsoleAsync($"Executing Periodical Event: {type.Name}...");
    }

    internal static async Task LogExceptionAsync(Exception exception)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        await Console.Out.WriteLineAsync(BuildConsoleExceptionMessage(exception));

        Console.ForegroundColor = ConsoleColor.Gray;

        if (TelegramBotModuleFramework.TelegramBotOptions.IsSendExceptionsToFirstAdministrator)
            await TelegramBotModuleFramework.TelegramBotClient.SendTextMessageAsync(TelegramBotModuleFramework.TelegramBotOptions.AdministratorUserIds[0], BuildTelegramExceptionMessage(exception), parseMode: ParseMode.Markdown);
    }

    private static string BuildConsoleExceptionMessage(Exception exception)
    {
        var delimiter = new string('-', 30);

        var sb = new StringBuilder();

        sb.AppendLine(delimiter)
            .AppendLine("[TBMF]: Exception!")
            .Append("[Type]: ")
            .Append(exception.GetType())
            .AppendLine()
            .Append("[Message]: ")
            .AppendLine(exception.Message)
            .Append("[Inner Exception]: ")
            .AppendLine(exception.InnerException is not null ? exception.InnerException.Message : "None")
            .Append("[Stack Trace]: ")
            .AppendLine(exception.StackTrace)
            .AppendLine(delimiter);

        return sb.ToString();
    }

    private static string BuildTelegramExceptionMessage(Exception exception)
    {
        var delimiter = new string('-', 30);

        var sb = new StringBuilder();

        sb.AppendLine(delimiter)
            .AppendLine("*[TBMF] Exception! 📮*")
            .AppendLine(delimiter)
            .Append("*Type*: _")
            .Append(exception.GetType())
            .AppendLine("_")
            .Append("*Message*: _")
            .Append(exception.Message)
            .AppendLine("_")
            .Append("*Inner Exception*: _")
            .Append(exception.InnerException is not null ? exception.InnerException.Message : "None")
            .AppendLine("_")
            .Append("```")
            .Append(exception.StackTrace)
            .AppendLine("```");

        return sb.ToString();
    }
}