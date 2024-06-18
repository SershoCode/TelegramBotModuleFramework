using System.Text;
using TBMF.Core;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApplication.Commands;

public class StatsCommand : ITelegramCommand
{
    public async Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update)
    {
        var messageEntityRepository = new MessageEntityRepository();

        var groupsForSendStats = messageEntityRepository.GetAllGroups();

        foreach (var groupId in groupsForSendStats)
        {
            var stringBuilder = new StringBuilder();

            var messagesCount = messageEntityRepository.GetMessagesCountByGroup(groupId);
            var wordsCount = messageEntityRepository.GetWordsCountByGroup(groupId);

            var topUserByMessages = messageEntityRepository.GetTopUserByMessageType(groupId, MessageType.Text);
            var topUserByStickers = messageEntityRepository.GetTopUserByMessageType(groupId, MessageType.Sticker);
            var topUserByPhoto = messageEntityRepository.GetTopUserByMessageType(groupId, MessageType.Photo);
            var topUserByVoice = messageEntityRepository.GetTopUserByMessageType(groupId, MessageType.Voice);
            var topUserByVideo = messageEntityRepository.GetTopUserByMessageType(groupId, MessageType.Video);
            var topUserByYoutubeShorts = messageEntityRepository.GetTopUserByMessagesСontains(groupId, "youtube.com/shorts");
            var topUserByLaungth = messageEntityRepository.GetTopUserByMessagesСontains(groupId, "хаха");

            var messages = messageEntityRepository.GetByExpression(messageEntity => messageEntity.ChatId == 123);

            if (messagesCount > 0)
            {
                stringBuilder
                    .AppendLine()
                    .AppendLine("🌟 *Статистика дня* 🌟")
                    .AppendLine($"Сообщений: *{messagesCount.Shortinize()}*");

                if (wordsCount > 0)
                {
                    stringBuilder
                        .AppendLine($"Слов: *{wordsCount.Shortinize()}*")
                        .AppendLine();
                }

                if (topUserByMessages is not null)
                {
                    stringBuilder
                        .AppendLine("*Флудер* 🤯:")
                        .AppendLine($"*{GetUserName(topUserByMessages)}: {topUserByMessages.Count.ToRussianQuantity("сообщение")} (*{Math.Round(topUserByMessages.Count / (messagesCount / 100d), 0)}%* от всех)")
                        .AppendLine();
                }

                if (topUserByStickers is not null)
                {
                    stringBuilder
                        .AppendLine("*Стикерист* 📝:")
                        .AppendLine($"*{GetUserName(topUserByStickers)}: {topUserByStickers.Count.ToRussianQuantity("стикер")}")
                        .AppendLine();
                }

                if (topUserByPhoto is not null)
                {
                    stringBuilder
                        .AppendLine("*Мемолог* 🌁:")
                        .AppendLine($"*{GetUserName(topUserByPhoto)}: {topUserByPhoto.Count.ToRussianQuantity("картинка")}")
                        .AppendLine();
                }

                if (topUserByVoice is not null)
                {
                    stringBuilder
                        .AppendLine("*Войсер* 🎙:")
                        .AppendLine($"*{GetUserName(topUserByVoice)}: {topUserByVoice.Count.ToRussianQuantity("голосовое")}")
                        .AppendLine();
                }

                if (topUserByVideo is not null)
                {
                    stringBuilder
                        .AppendLine("*Загрузчик видосов* 📹:")
                        .AppendLine($"*{GetUserName(topUserByVideo)}: {topUserByVideo.Count.ToRussianQuantity("видео")}")
                        .AppendLine();
                }

                if (topUserByYoutubeShorts is not null)
                {
                    stringBuilder
                        .AppendLine("*Любитель шортсов* 🎞:")
                        .AppendLine($"*{GetUserName(topUserByYoutubeShorts)}: {topUserByYoutubeShorts.Count.ToRussianQuantity("шортс")}")
                        .AppendLine();
                }

                if (topUserByLaungth is not null)
                {
                    stringBuilder
                        .AppendLine("*Смешинка в рот попала* 😂:")
                        .AppendLine($"*{GetUserName(topUserByLaungth)}: {topUserByLaungth.Count} хахахов")
                        .AppendLine();
                }

                await botClient.SendTextMessageAsync(groupId, stringBuilder.ToString(), parseMode: ParseMode.Markdown);

                if (botOptions.IsDropStatsAfterSend)
                    await messageEntityRepository.ClearTableAsync();
            }
        }
    }

    private static string GetUserName(TopUserInfoDto userInfo)
    {
        return $"{userInfo.UserFirstName} {userInfo.UserLastName}*{(!string.IsNullOrEmpty(userInfo.UserName) ? $"(@{userInfo.UserName})" : string.Empty)}";
    }
}
