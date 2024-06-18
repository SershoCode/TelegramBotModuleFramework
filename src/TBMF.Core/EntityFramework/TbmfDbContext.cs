using Microsoft.EntityFrameworkCore;

namespace TBMF.Core;

internal sealed class TbmfDbContext : DbContext
{
    private const string DbName = "tbmf.db";

    internal DbSet<CommandStateEntity> CommandStates { get; set; }
    internal DbSet<MessageEntity> Messages { get; set; }

    internal TbmfDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var options = TelegramBotModuleFramework.TelegramBotOptions;

        if (options.IsUseSqlite)
        {
            var dbFolder = TelegramBotModuleFramework.TelegramBotOptions.SqliteDbFolderPath;

            if (dbFolder is not null && !Directory.Exists(dbFolder))
                Directory.CreateDirectory(dbFolder);

            optionsBuilder.UseSqlite($"Data Source={dbFolder}/{DbName};")
                .EnableDetailedErrors(true);
        }
        else if (options.IsUsePostgres)
        {
            optionsBuilder.UseNpgsql(options.PgConnectionString);
        }
    }
}