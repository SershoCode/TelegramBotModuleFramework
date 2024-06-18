using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBMF.Core;

internal class ControlCommand : ITelegramSystemCommand
{
    internal static List<List<InlineKeyboardButton>> CommandKeyboard = [];

    public async Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update)
    {
        CommandKeyboard = [];

        var repo = new CommandStateEntityRepository();

        var typeStates = await repo.GetListAsync();

        var sb = new StringBuilder();

        var delimiter = new string('-', 30);

        sb.AppendLine("Список команд бота:");
        sb.AppendLine(delimiter);

        foreach (var typeState in typeStates)
        {
            sb.Append('*')
                .Append(typeState.Name)
                .Append("*: ")
                .AppendLine(typeState.IsEnabled ? "Enabled ✅" : "Disabled ⭕️");

            var buttonText = $"[Toggle] {typeState.Name}";

            CommandKeyboard.Add([InlineKeyboardButton.WithCallbackData(text: buttonText, callbackData: new ToggleCommandCallBackDto(typeState.Name).ToString())]);
        }

        sb.AppendLine(delimiter);

        var keyboard = new InlineKeyboardMarkup(CommandKeyboard);

        await botClient.SendTextMessageAsync(update.Message.Chat.Id, sb.ToString().TrimEnd('\n'), parseMode: ParseMode.Markdown, replyMarkup: keyboard);
    }
}