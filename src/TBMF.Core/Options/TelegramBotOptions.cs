namespace TBMF.Core;

public class TelegramBotOptions
{
    public required string Token { get; set; }
    public required List<long> AdministratorUserIds { get; set; }
    public required bool IsSendExceptionsToFirstAdministrator { get; set; }
    public bool IsCollectStats { get; set; }
    public bool IsDropStatsAfterSend { get; set; }

    internal string SqliteDbFolderPath { get; set; } = "db";
    internal string PgConnectionString { get; set; }
    internal bool IsUseSqlite { get; set; }
    internal bool IsUsePostgres { get; set; }
    internal bool IsEnsureCreated { get; set; }

    public TelegramBotOptions UseSqlite(string dbFolderPath, bool ensureCreated = false)
    {
        if (IsUsePostgres)
            throw new TbmfDbException("Postgres already selected, please select one database to use");

        IsUseSqlite = true;

        SqliteDbFolderPath = dbFolderPath;

        IsEnsureCreated = ensureCreated;

        return this;
    }

    public TelegramBotOptions UsePostgres(string dbAddress, int dbPort, string dbName, string dbUser, string dbPassword, bool ensureCreated = false)
    {
        if (IsUseSqlite)
            throw new TbmfDbException("Sqlite already selected, please select one database to use");

        IsUsePostgres = true;

        PgConnectionString = $"User ID={dbUser};Password={dbPassword};Host={dbAddress};Port={dbPort};Database={dbName};Pooling=true;Connection Lifetime=0;";

        IsEnsureCreated = ensureCreated;

        return this;
    }
}