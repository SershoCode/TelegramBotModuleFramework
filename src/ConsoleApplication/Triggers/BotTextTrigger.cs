using TBMF.Core;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApplication.Triggers;

public class BotTextTrigger : ITelegramTextTrigger
{
    public IEnumerable<string> Triggers { get; set; } = ["бот"];
    public TextTriggerType TextTriggerType { get; set; } = TextTriggerType.Equals;
    public int TextTriggerChancePercentage { get; set; } = 50;

    private readonly List<string> _answers = ["Здесь", "К вашим услугам", "Я вас слушаю"];

    public async Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update)
    {
        var randomAnswer = _answers[StaticRandom.Instance.Next(_answers.Count)];

        await botClient.SendTextMessageAsync(update.Message.Chat.Id, randomAnswer, replyToMessageId: update.Message.MessageId);
    }
}