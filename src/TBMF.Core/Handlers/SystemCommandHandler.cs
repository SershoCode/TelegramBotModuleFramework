using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBMF.Core;

internal static class SystemCommandHandler
{
    internal static async Task HandleAsync(string commandTrigger, Update update, CancellationToken cancellationToken)
    {
        commandTrigger = commandTrigger.Replace("/", "");

        await TbmfLogger.LogCommandToConsoleAsync(commandTrigger, update);

        if (!TelegramBotModuleFramework.TelegramBotOptions.AdministratorUserIds.Contains(update.Message.From.Id))
        {
            await TelegramBotModuleFramework.TelegramBotClient.SendTextMessageAsync(update.Message.Chat.Id,
                "Извините, вы не являетесь одним из администраторов бота.", replyToMessageId: update.Message.MessageId, cancellationToken: cancellationToken);

            return;
        }

        var commandType = TelegramBotModuleFramework.SystemCommandTypes.First(command => command.Name.StartsWith(commandTrigger, StringComparison.CurrentCultureIgnoreCase));

        var commandInstance = (ITelegramSystemCommand)Activator.CreateInstance(commandType);

        await commandInstance.ExecuteAsync(TelegramBotModuleFramework.TelegramBotClient, TelegramBotModuleFramework.TelegramBotOptions, update);
    }
}