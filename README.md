# TelegramBotModuleFramework

*TBMF* - Платформа для удобной разработки ботов Telegram.

Если вам не хочется заморачиваться с настройкой, получением сообщений, определением того, что за команда пришла, вы можете воспользоваться платформой и ваш бот будет готов уже через минуту.

## Возможности

### Простая инициализация.

Достаточно лишь создать консольный проект, сделать несколько строк в Program.cs и ваш бот готов к работе, уже можно запускаться.

```csharp
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
```

И на этом всё, можно добавлять свои команды и использовать другие возможности, которые будут описаны ниже.

### Автоматическое определение текстовых команд.

Вы можете создать класс, реализовать нужный интерефейс и лишь написать бизнес-логику, а платформа все сделает за вас.

```csharp
[Access(000, 111)]
[TgDescription("Receive Pong!")]
public class PingCommand : ITelegramCommand
{
    public async Task ExecuteAsync(ITelegramBotClient botClient, TelegramBotOptions botOptions, Update update)
    {
        await botClient.ReplyToMessageAsync(update.Message, "*Pong!*");
    }
}
```

Вам нужно лишь создать класс с названием команды и имплементировать интерфейс ITelegramCommand.

Команда сама будет добавлена в бота по имени класса, в нашем примере это соответственно /ping.

Имплементация предоставит вам метод ExecuteAsync, в который уже будут прокинуты настройки, бот-клиент телеграма и само Update событие.

Так же, с помощью аттрибутов, вы можете задать текстовое описание команды, которое будет отображаться в телеграме при вызове "/" и доступ к исполнению команды для конкретных пользователей.

Если не использовать аттрибуты, то поведение будет по умолчанию: Без описания, доступно всем.

### Реакция на текстовые события.

TBMF предоставляет возможность реагировать на текст сообщений с определенным шансом и типом.

```csharp
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
```

Вы можете задать шанс срабатывания, тип перехвата для срабатывания (сообщение содержит/начинается/оканчивается/полностью равно).

### Периодические ивенты.

В TBMF встроен Hangfire. Вы можете создать класс, задать ему тайминг исполнения и он будет выполняться в нужное время.
Например, это можно использовать для отправки ежедневной статистики, погоды и так далее. Или раз в час писать что нибудь.
Поддерживаются как и заданные периоды, так и Cron-выражения в виде строк.

Библиотека умеет поддерживать статус своих модулей через базу данных, поэтому после выполнения пайплайна CI/CD ваши настройки не слетят.

### Панель администратора

Вы можете управлять включением и отключением команд и ивентов бота с помощью панели администратора.

Так же при запуске бот вас уведомит о том, какие команды и возможности сейчас активны.
