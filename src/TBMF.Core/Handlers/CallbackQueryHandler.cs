using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TBMF.Core;

internal static class CallbackQueryHandler
{
    private static ITelegramBotClient BotClient;
    private static Update Update;
    private static CallbackQuery CallbackQuery;

    internal static async Task HandleAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        BotClient = botClient;
        Update = update;
        CallbackQuery = update.CallbackQuery;

        var callbackDtoName = GetCallbackDataByIndex(0);

        switch (callbackDtoName)
        {
            case nameof(ToggleCommandCallBackDto):
                {
                    await HandleCommandToggleAsync(GetCallbackDataByIndex(1), cancellationToken);
                    break;
                }
        }
    }

    private static async Task HandleCommandToggleAsync(string commandName, CancellationToken cancellationToken)
    {
        if (!TelegramBotModuleFramework.TelegramBotOptions.AdministratorUserIds.Contains(CallbackQuery.From.Id))
            await BotClient.AnswerCallbackQueryAsync(CallbackQuery.Id, "Access Denied", cancellationToken: cancellationToken);

        var repository = new CommandStateEntityRepository();

        var typeState = await repository.FindByNameAsync(commandName);

        if (typeState != null)
        {
            typeState.Toggle();

            typeState.RefreshUpdatedTime();

            await repository.SaveChangesAsync();

            if (HangfireManager.TypeIsPeriodicEvent(typeState.Name))
            {
                var periodicEvent = TelegramBotModuleFramework.EventTypes.First(type => type.Name == typeState.Name);

                await HangfireManager.RestartJob(periodicEvent);
            }

            var oldMessageText = CallbackQuery.Message.Text;

            var enabledText = $"{commandName}: Enabled ✅";
            var disabledText = $"{commandName}: Disabled ⭕️";

            var newMessageText = oldMessageText.Contains(enabledText) ?
                oldMessageText.Replace(enabledText, $"*{disabledText}*") :
                oldMessageText.Replace(disabledText, $"*{enabledText}*");

            await BotClient.EditMessageTextAsync(CallbackQuery.Message.Chat.Id, CallbackQuery.Message.MessageId,
                newMessageText, parseMode: ParseMode.Markdown, replyMarkup: new InlineKeyboardMarkup(ControlCommand.CommandKeyboard), cancellationToken: cancellationToken);
        }
    }

    private static string GetCallbackDataByIndex(int index)
    {
        return CallbackQuery.Data.Split(':')[index];
    }
}