using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TBMF.Core;

public class MessageEntityRepository : IRepository<MessageEntity>
{
    private readonly TbmfDbContext _dbContext;

    public MessageEntityRepository()
    {
        _dbContext = new TbmfDbContext();
    }

    public async Task AddEntityAsync(MessageEntity entity)
    {
        await _dbContext.Messages.AddAsync(entity);

        await _dbContext.SaveChangesAsync();
    }

    public async Task AddEntitiesAsync(IEnumerable<MessageEntity> entities)
    {
        await _dbContext.Messages.AddRangeAsync(entities);

        await _dbContext.SaveChangesAsync();
    }

    public List<long> GetAllGroups()
    {
        var groups = _dbContext.Messages
            .AsNoTracking()
            .Where(messageEntity => messageEntity.ChatType == ChatType.Group || messageEntity.ChatType == ChatType.Supergroup)
            .Select(messageEntity => messageEntity.ChatId)
            .Distinct()
            .ToList();

        return groups;
    }

    public List<MessageEntity> GetByExpression(Expression<Func<MessageEntity, bool>> expression)
    {
        return _dbContext.Messages
            .AsNoTracking()
            .Where(expression)
            .ToList();
    }

    public int GetMessagesCountByGroup(long chatId, int hours = 24)
    {
        var messagesCount = _dbContext.Messages
            .AsNoTracking()
            .Count(messageEntity => messageEntity.ChatId == chatId && messageEntity.SendedAt >= SubtractDateFromCurrent(hours));

        return messagesCount;
    }

    public DateTime GetLastMessageTimeInGroup(long chatId)
    {
        var lastMessageTime = _dbContext.Messages
            .AsNoTracking()
            .Where(messageEntity => messageEntity.ChatId == chatId)
            .Select(messageEntity => messageEntity.SendedAt)
            .OrderByDescending(sendedAt => sendedAt)
            .FirstOrDefault();

        return lastMessageTime;
    }

    public TimeSpan GetDifferenceBetweenLastMessageInChatAndCurrentDateTime(long chatId)
    {
        var lastMessageTimeInChat = GetLastMessageTimeInGroup(chatId);

        return DateTime.UtcNow - lastMessageTimeInChat;
    }

    public List<string> GetAllUniqueWordsByGroup(long chatId, int hours = 24)
    {
        var userMessages = _dbContext.Messages
            .AsNoTracking()
            .Where(messageEntity => messageEntity.ChatId == chatId && messageEntity.MessageType == MessageType.Text && messageEntity.SendedAt >= SubtractDateFromCurrent(hours))
            .Select(messageEntity => messageEntity.MessageText)
            .ToList();

        var userWords = userMessages.SelectMany(messsage => messsage.Split(' '))
            .Select(word => word.RemoveSpecialCharacters())
            .Select(messageEntity => messageEntity.ToLower())
            .Distinct()
            .ToList();

        return userWords;
    }

    public TopUserInfoDto GetTopUserByMessagesContains(long chatId, IEnumerable<string> words, int hours = 24)
    {
        var userMessages = _dbContext.Messages
            .AsNoTracking()
            .Where(messageEntity => messageEntity.ChatId == chatId && messageEntity.MessageType == MessageType.Text && messageEntity.SendedAt >= SubtractDateFromCurrent(hours))
            .Select(messageEntity => new { messageEntity.UserId, messageEntity.UserName, messageEntity.UserFirstName, messageEntity.UserLastName, messageEntity.MessageText })
            .ToList();

        var messagesContainsWords = userMessages.Where(messageEntity => messageEntity.MessageText.Split(' ').Any(word => words.Contains(word, StringComparer.InvariantCultureIgnoreCase)));

        var countDictionary = new Dictionary<long, int>();

        foreach (var message in messagesContainsWords)
        {
            var containsCount = message.MessageText.Split(' ').Count(word => words.Contains(word, StringComparer.InvariantCultureIgnoreCase));

            if (!countDictionary.TryAdd(message.UserId, containsCount))
                countDictionary[message.UserId] += containsCount;
        }

        if (countDictionary.Count != 0)
        {
            var topUserFromDictionary = countDictionary.OrderByDescending(x => x.Value).First();
            var topUserInfo = userMessages.First(userMessage => userMessage.UserId == topUserFromDictionary.Key);

            var infoDto = new TopUserInfoDto
            {
                UserName = topUserInfo.UserName ?? string.Empty,
                UserFirstName = topUserInfo.UserFirstName ?? string.Empty,
                UserLastName = topUserInfo.UserLastName ?? string.Empty,
                Count = topUserFromDictionary.Value
            };

            return infoDto;
        }

        return null;
    }

    public TopUserInfoDto GetTopUserByMessagesСontains(long chatId, string containsText, int hours = 24)
    {
        var topUser = _dbContext.Messages
            .AsNoTracking()
            .Where(messageEntity => messageEntity.ChatId == chatId && messageEntity.MessageType == MessageType.Text && messageEntity.SendedAt >= SubtractDateFromCurrent(hours) && messageEntity.MessageText.Contains(containsText))
            .GroupBy(messageEntity => messageEntity.UserId)
            .Select(user => new { user.Key, MessagesCount = user.Count() })
            .OrderBy(user => user.MessagesCount)
            .LastOrDefault();

        if (topUser is not null)
        {
            var topUserInfo = _dbContext.Messages.First(messageEntity => messageEntity.UserId == topUser.Key);

            var infoDto = new TopUserInfoDto
            {
                UserName = topUserInfo.UserName ?? string.Empty,
                UserFirstName = topUserInfo.UserFirstName ?? string.Empty,
                UserLastName = topUserInfo.UserLastName ?? string.Empty,
                Count = topUser.MessagesCount
            };

            return infoDto;
        }

        return null;
    }

    public TopUserInfoDto GetTopUserByMessageType(long chatId, MessageType messageType, int hours = 24)
    {
        var topUser = _dbContext.Messages
            .AsNoTracking()
            .Where(messageEntity => messageEntity.ChatId == chatId && messageEntity.MessageType == messageType && messageEntity.SendedAt >= SubtractDateFromCurrent(hours))
            .GroupBy(messageEntity => messageEntity.UserId)
            .Select(user => new { user.Key, MessagesCount = user.Count() })
            .OrderBy(user => user.MessagesCount)
            .LastOrDefault();

        if (topUser is not null)
        {
            var topUserInfo = _dbContext.Messages.First(messageEntity => messageEntity.UserId == topUser.Key);

            var infoDto = new TopUserInfoDto
            {
                UserName = topUserInfo.UserName ?? string.Empty,
                UserFirstName = topUserInfo.UserFirstName ?? string.Empty,
                UserLastName = topUserInfo.UserLastName ?? string.Empty,
                Count = topUser.MessagesCount
            };

            return infoDto;
        }

        return null;
    }
    public int GetWordsCountByGroup(long chatId, int hours = 24)
    {
        var messagesText = _dbContext.Messages
            .AsNoTracking()
            .Where(messageEntity => messageEntity.MessageText != null && messageEntity.ChatId == chatId && messageEntity.SendedAt >= SubtractDateFromCurrent(hours))
            .Select(messageEntity => messageEntity.MessageText);

        var sum = messagesText
            .ToList()
            .Where(messagesText => messagesText is not null)
            .Select(messageText => messageText.Split(' '))
            .Sum(separatedText => separatedText.Length);

        return sum;
    }

    public async Task ClearTableAsync()
    {
        await TbmfLogger.LogToConsoleAsync("Wiping stats table...");

        TbmfStatsMessageEntityDbCache.StopWritingCacheToDatabase();

        await Task.Delay(TimeSpan.FromSeconds(TbmfStatsMessageEntityDbCache.WritePeriodicSeconds));

        if (TelegramBotModuleFramework.TelegramBotOptions.IsUseSqlite)
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Messages");

        if (TelegramBotModuleFramework.TelegramBotOptions.IsUsePostgres)
            await _dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Messages");

        TbmfStatsMessageEntityDbCache.StartWritingCacheToDatabase();

        await TbmfLogger.LogToConsoleAsync("Wiping stats table success");
    }

    private static DateTime SubtractDateFromCurrent(int hours = 24) => DateTime.UtcNow.Subtract(TimeSpan.FromHours(hours));
}
