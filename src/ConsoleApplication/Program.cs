using TBMF.Core;

namespace ConsoleApplication;

public static class Program
{
    public static async Task Main()
    {
        await TelegramBotModuleFramework.RunAsync(new TelegramBotOptions
        {
            Token = "BotToken",
            AdministratorUserIds = [000000, 11111],
            IsSendExceptionsToFirstAdministrator = true,
            IsCollectStats = true,
            IsDropStatsAfterSend = false,
        }.UsePostgres("192.168.1.1", 5432, "db", "user", "password"));

        await Task.Run(async () => await Task.Delay(Timeout.Infinite));
    }
}
