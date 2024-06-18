using Microsoft.Extensions.Primitives;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBMF.Core;

public static class TelegramBotModuleFramework
{
    internal static readonly IEnumerable<Type> EventTypes;
    internal static readonly IEnumerable<Type> TriggerTypes;
    internal static readonly IEnumerable<Type> CommandTypes;
    internal static readonly IEnumerable<Type> SystemCommandTypes;
    internal static readonly IEnumerable<string> CommandTriggers;
    internal static readonly IEnumerable<string> SystemCommandTriggers;
    internal static readonly Dictionary<Type, IEnumerable<string>> TriggerBinds;

    internal static ITelegramBotClient TelegramBotClient { get; private set; }
    internal static TelegramBotOptions TelegramBotOptions { get; private set; }
    internal static CancellationToken CancellationToken { get; }

    static TelegramBotModuleFramework()
    {
        EventTypes = FindAllTypesByInterface<ITelegramPeriodicEvent>();
        TriggerTypes = FindAllTypesByInterface<ITelegramTextTrigger>();
        TriggerBinds = BindAllTriggersToTypes();
        CommandTypes = FindAllTypesByInterface<ITelegramCommand>();
        CommandTriggers = FindCommandTriggers(CommandTypes);
        SystemCommandTypes = FindAllTypesByInterface<ITelegramSystemCommand>();
        SystemCommandTriggers = FindCommandTriggers(SystemCommandTypes);
        CancellationToken = new CancellationToken();
    }

    public static async Task RunAsync(TelegramBotOptions options)
    {
        var telegramBotClient = new TelegramBotClient(options.Token);

        TelegramBotClient = telegramBotClient;

        TelegramBotOptions = options;

        await PersistentDatabaseAsync();

        var botInfo = await telegramBotClient.GetMeAsync();

        await telegramBotClient.SendTextMessageAsync(options.AdministratorUserIds[0], BuildStartMessage(botInfo.Username), parseMode: ParseMode.Markdown);

        telegramBotClient.StartReceiving<TelegramBotUpdateHandler>(new ReceiverOptions { ThrowPendingUpdates = true });

        await SetCommandDescriptionsAsync();

        await StartPeriodicJobs();

        StartCollectingStats();

        await TbmfLogger.LogToConsoleAsync($"{botInfo.Username} Running!");
    }

    private static string BuildStartMessage(string botName)
    {
        var delimiter = new string('-', 10);

        var runningText = new StringBuilder();

        runningText
            .Append("```").AppendLine(botName)
            .Append("Hello! I'm started and founded ")
            .Append(EventTypes.Count() + TriggerTypes.Count() + CommandTypes.Count() + SystemCommandTypes.Count() )
            .AppendLine(" actions!")
            .AppendLine(delimiter)
            .AppendLine("[Text Commands]:")
            .AppendLine(GetTypeListFromCollection(CommandTypes))
            .AppendLine("[Text Triggers]:")
            .AppendLine(GetTypeListFromCollection(TriggerTypes))
            .AppendLine("[Periodically Events]:")
            .AppendLine(GetTypeListFromCollection(EventTypes))
            .AppendLine("[Admin Commands]:")
            .AppendLine(GetTypeListFromCollection(SystemCommandTypes))
            .AppendLine(delimiter)
            .AppendLine("I'm Ready!")
            .AppendLine("Thanks for using TBMF ❤️")
            .AppendLine("```");

        return runningText.ToString();
    }

    private static async Task StartPeriodicJobs()
    {
        await HangfireManager.AddJobs(EventTypes);
    }

    private static async Task SetCommandDescriptionsAsync()
    {
        var commandsWithDescriptionAttribute = GetTypesWithAttribute<TgDescriptionAttribute>(CommandTypes);

        var commands = new List<BotCommand>();

        foreach (var command in commandsWithDescriptionAttribute)
        {
            var attribute = (TgDescriptionAttribute)command.GetCustomAttributes(false).SingleOrDefault(att => att is TgDescriptionAttribute);

            commands.Add(new BotCommand()
            {
                Command = command.Name.Replace("Command", "", StringComparison.InvariantCultureIgnoreCase).ToLower(),
                Description = attribute.CommandDescription
            });
        }

        await TelegramBotClient.DeleteMyCommandsAsync();

        await Task.Delay(TimeSpan.FromSeconds(1));

        await TelegramBotClient.SetMyCommandsAsync(commands);
    }

    private static IEnumerable<Type> FindAllTypesByInterface<TInterface>()
    {
        var type = typeof(TInterface);

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assemblies => assemblies.GetTypes())
            .Where(tp => type.IsAssignableFrom(tp) && tp.IsClass);

        return types;
    }

    private static IEnumerable<string> FindCommandTriggers(IEnumerable<Type> collection)
    {
        return collection.Select(command => $"/{command.Name.Replace("Command", "").ToLower()}");
    }

    private static IEnumerable<Type> GetTypesWithAttribute<TType>(IEnumerable<Type> typeCollection)
    {
        var res = typeCollection.Where(tp => (TType)tp.GetCustomAttributes(false).SingleOrDefault(att => att is TType) != null);

        return res;
    }

    private static string GetTypeListFromCollection(IEnumerable<Type> collection)
    {
        var sb = new StringBuilder();

        foreach (var eventType in collection)
        {
            sb.Append("  - ").AppendLine(eventType.Name);
        }

        var res = sb.ToString()
            .TrimEnd('\n');

        return string.IsNullOrEmpty(res) ? "  - None" : res;
    }

    private static Dictionary<Type, IEnumerable<string>> BindAllTriggersToTypes()
    {
        var res = new Dictionary<Type, IEnumerable<string>>();

        foreach (var triggerType in TriggerTypes)
        {
            var tempInstance = (ITelegramTextTrigger)Activator.CreateInstance(triggerType);

            res.Add(triggerType, tempInstance.Triggers);
        }

        return res;
    }

    private static async Task PersistentDatabaseAsync()
    {
        var allTypes = new List<Type>();

        allTypes.AddRange(CommandTypes);
        allTypes.AddRange(TriggerTypes);
        allTypes.AddRange(EventTypes);

        var repo = new CommandStateEntityRepository();

        // Удаляем все стейты, которых больше нет в приложении.
        await repo.DeleteNotFoundAsync(allTypes);

        // Добавляем все стейты, которые есть в приложении, но нет в базе.
        await repo.AddMissingAsync(allTypes);
    }

    private static void StartCollectingStats()
    {
        if (TelegramBotOptions.IsCollectStats)
        {
            TbmfStatsMessageEntityDbCache.StartWritingCacheToDatabase();
        }
    }
}