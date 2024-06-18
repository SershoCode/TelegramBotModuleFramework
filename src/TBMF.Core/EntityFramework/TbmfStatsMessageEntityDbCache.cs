namespace TBMF.Core;

internal static class TbmfStatsMessageEntityDbCache
{
    private static readonly SynchronizedCollection<MessageEntity> _messageEntities;
    internal static CancellationTokenSource CancellationTokenSource;
    internal const int WritePeriodicSeconds = 5;

    static TbmfStatsMessageEntityDbCache()
    {
        _messageEntities = [];
        CancellationTokenSource = new();
    }

    internal static void AddMessageEntity(MessageEntity messageEntity)
    {
        _messageEntities.Add(messageEntity);
    }

    internal static void StartWritingCacheToDatabase()
    {
        CancellationTokenSource = new();

        WriteCacheToDatabase();
    }

    internal static void StopWritingCacheToDatabase()
    {
        CancellationTokenSource.Cancel();
    }

    private static void WriteCacheToDatabase()
    {
        var messageEntityRepository = new MessageEntityRepository();

        Task.Factory.StartNew(async () =>
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                if (_messageEntities.Count != 0)
                {
                    await TbmfLogger.LogToConsoleAsync($"Writing {_messageEntities.Count} {nameof(MessageEntity)} to db...");

                    await messageEntityRepository.AddEntitiesAsync(_messageEntities);

                    _messageEntities.Clear();
                }

                if (!CancellationTokenSource.IsCancellationRequested)
                    await Task.Delay(TimeSpan.FromSeconds(WritePeriodicSeconds));
            }
        }, TaskCreationOptions.LongRunning);
    }
}