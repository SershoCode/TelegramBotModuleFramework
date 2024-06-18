using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBMF.Core;

internal class TelegramBotUpdateHandler : IUpdateHandler
{
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update is null)
            return;

        if (update.Type == UpdateType.EditedMessage)
            return;

        if (update.Type == UpdateType.CallbackQuery)
            await CallbackQueryHandler.HandleAsync(botClient, update, cancellationToken);

        var updateMessage = update.Message;

        var messageText = updateMessage?.Text ?? "";

        if (!messageText.StartsWith('/') && updateMessage is { ForwardFrom: null } && !TbmfStatsMessageEntityDbCache.CancellationTokenSource.IsCancellationRequested)
        {
            var messageEntity = new MessageEntity()
            {
                Id = Guid.NewGuid(),
                ChatId = updateMessage.Chat.Id,
                ChatName = updateMessage.Chat.Title,
                ChatType = updateMessage.Chat.Type,
                UserId = updateMessage.From.Id,
                MessageId = updateMessage.MessageId,
                UserName = updateMessage.From.Username,
                UserFirstName = updateMessage.From.FirstName,
                UserLastName = updateMessage.From.LastName,
                MessageType = updateMessage.Type,
                MessageText = messageText,
                SendedAt = DateTime.UtcNow
            };

            TbmfStatsMessageEntityDbCache.AddMessageEntity(messageEntity);
        }

        // Received command.
        var commandTrigger = TelegramBotModuleFramework.CommandTriggers.FirstOrDefault(trigger => messageText.StartsWith(trigger));

        if (commandTrigger is not null)
        {
            await CommandHandler.HandleAsync(commandTrigger, update, cancellationToken);

            return;
        }

        // Received system command.
        var systemCommandTrigger = TelegramBotModuleFramework.SystemCommandTriggers.FirstOrDefault(trigger => messageText.StartsWith(trigger));

        if (systemCommandTrigger is not null)
        {
            await SystemCommandHandler.HandleAsync(systemCommandTrigger, update, cancellationToken);

            return;
        }

        // Received text trigger.
        var typesContainsTextTrigger = TelegramBotModuleFramework.TriggerBinds
            .Where(triggerBind => triggerBind.Value.Any(value => messageText.Contains(value, StringComparison.InvariantCultureIgnoreCase)))
            .Select(triggerBind => triggerBind.Key);

        if (typesContainsTextTrigger.Any())
        {
            await TextTriggerHandler.HandleAsync(typesContainsTextTrigger, update);
        }
    }

    public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        await TbmfLogger.LogExceptionAsync(exception);

        await Task.Delay(5000, cancellationToken);

        botClient.StartReceiving<TelegramBotUpdateHandler>(new ReceiverOptions() { ThrowPendingUpdates = true }, cancellationToken: cancellationToken);
    }
}