using Telegram.Bot;
using Telegram.Bot.Types;

namespace TBMF.Core;

internal static class CommandHandler
{
    internal static async Task HandleAsync(string commandTrigger, Update update, CancellationToken cancellationToken)
    {
        commandTrigger = commandTrigger.Replace("/", "");

        await TbmfLogger.LogCommandToConsoleAsync(commandTrigger, update);

        var commandType = TelegramBotModuleFramework.CommandTypes.First(command => command.Name.StartsWith(commandTrigger, StringComparison.CurrentCultureIgnoreCase));

        var isCommandEnabled = await CommandStateManager.IsCommandEnabled(commandType);

        if (!isCommandEnabled)
        {
            await TelegramBotModuleFramework.TelegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Извините, данная команда отключена администратором.", cancellationToken: cancellationToken);

            return;
        }

        var accessAttribute = (AccessAttribute)commandType.GetCustomAttributes(false).SingleOrDefault(att => att is AccessAttribute);

        if (accessAttribute?.Users.Contains(update.Message.From.Id) != false)
        {
            var commandInstance = (ITelegramCommand)Activator.CreateInstance(commandType);

            await commandInstance.ExecuteAsync(TelegramBotModuleFramework.TelegramBotClient, TelegramBotModuleFramework.TelegramBotOptions, update);
        }
        else
        {
            await TelegramBotModuleFramework.TelegramBotClient.SendTextMessageAsync(update.Message.Chat.Id, $"User @{update.Message.From.Username} has no access to command: /{commandTrigger}", cancellationToken: cancellationToken);
        }
    }
}