using System.Text;
using TBMF.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApplication.Commands;

public class WordsCommand : ITelegramCommand
{
    public async Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update)
    {
        var messageEntityRepository = new MessageEntityRepository();

        var allGroups = messageEntityRepository.GetAllGroups();

        const int lastHours = 300;

        var stringBuilder = new StringBuilder();

        foreach (var group in allGroups)
        {
            var words = messageEntityRepository.GetAllUniqueWordsByGroup(group, lastHours);

            stringBuilder.AppendLine($"Group (*{group}*): *{words.Count()}* messages");
        }

        await botClient.ReplyToMessageAsync(update.Message, stringBuilder.ToString());
    }
}