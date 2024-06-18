using Hangfire;

namespace TBMF.Core;

internal static class HangfireManager
{
    static HangfireManager()
    {
        ConfigureHangfire();
    }

    internal static async Task AddJob(Type type)
    {
        var repo = new CommandStateEntityRepository();

        var jobIsEnabled = await repo.IsTypeEnabled(type);

        if (jobIsEnabled)
        {
            var tempTypeInstance = (ITelegramPeriodicEvent)Activator.CreateInstance(type);

            RecurringJob.AddOrUpdate(tempTypeInstance?.GetType().Name, () => RunJob(type),
                tempTypeInstance?.CronExpression);
        }
    }

    internal static async Task AddJobs(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            await AddJob(type);
        }
    }

    internal static async Task RestartJob(Type type)
    {
        RemoveJobIfExists(type.Name);

        await AddJob(type);
    }

    internal static void RemoveJobIfExists(string typeName)
    {
        RecurringJob.RemoveIfExists(typeName);
    }

    internal static bool TypeIsPeriodicEvent(string typeName)
    {
        return TelegramBotModuleFramework.EventTypes.Select(type => type.Name).Contains(typeName);
    }

    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public static async Task RunJob(Type type)
    {
        try
        {
            await TbmfLogger.LogPeriodicalEventToConsoleAsync(type);

            var typeInstance = (ITelegramPeriodicEvent)Activator.CreateInstance(type);

            await typeInstance.ExecuteAsync(TelegramBotModuleFramework.TelegramBotClient, TelegramBotModuleFramework.TelegramBotOptions);
        }
        catch (Exception ex)
        {
            await TbmfLogger.LogExceptionAsync(ex);
        }
    }

    private static BackgroundJobServer ConfigureHangfire()
    {
        GlobalConfiguration.Configuration.UseInMemoryStorage();

        return new BackgroundJobServer();
    }
}