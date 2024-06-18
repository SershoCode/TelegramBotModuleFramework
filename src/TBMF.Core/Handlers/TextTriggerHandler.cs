using Telegram.Bot.Types;

namespace TBMF.Core;

internal static class TextTriggerHandler
{
    internal static async Task HandleAsync(IEnumerable<Type> types, Update update)
    {
        foreach (var type in types)
        {
            var isEnabled = await CommandStateManager.IsCommandEnabled(type);

            if (!isEnabled)
                continue;

            var instance = (ITelegramTextTrigger)Activator.CreateInstance(type);

            var messageText = update.Message.Text;

            var triggers = (IEnumerable<string>)type.GetProperty("Triggers").GetValue(instance, null);

            if (!triggers.Any())
                return;

            var enumValue = (TextTriggerType)type.GetProperty(nameof(TextTriggerType)).GetValue(instance, null);
            var chance = (int)type.GetProperty("TextTriggerChancePercentage").GetValue(instance, null);
  
            if (chance < 100)
            {
                var isSuccess = StaticRandom.Instance.Next(0, 100) <= chance;

                if (!isSuccess)
                    return;
            }


            switch (enumValue)
            {
                case TextTriggerType.StartsWith:
                    {
                        if (triggers.Any(trigger => messageText.StartsWith(trigger, StringComparison.InvariantCultureIgnoreCase)))
                            await ExecuteInstance(instance, update);

                        break;
                    }
                case TextTriggerType.EndsWith:
                    {
                        if (triggers.Any(trigger => messageText.EndsWith(trigger, StringComparison.InvariantCultureIgnoreCase)))
                            await ExecuteInstance(instance, update);

                        break;
                    }
                case TextTriggerType.Contains:
                    {
                        if (triggers.Any(trigger => messageText.Contains(trigger, StringComparison.InvariantCultureIgnoreCase)))
                            await ExecuteInstance(instance, update);

                        break;
                    }
                case TextTriggerType.Equals:
                    {
                        if (triggers.Any(trigger => messageText.Equals(trigger, StringComparison.InvariantCultureIgnoreCase)))
                            await ExecuteInstance(instance, update);

                        break;
                    }
            }

            await TbmfLogger.LogTriggerToConsoleAsync(type, update);
        }
    }

    private static async Task ExecuteInstance(ITelegramTextTrigger textTriggerInstance, Update update)
    {
        await textTriggerInstance.ExecuteAsync(TelegramBotModuleFramework.TelegramBotClient, TelegramBotModuleFramework.TelegramBotOptions, update);
    }
}