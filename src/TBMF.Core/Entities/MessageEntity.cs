using Telegram.Bot.Types.Enums;

namespace TBMF.Core;

public class MessageEntity
{
    public required Guid Id { get; set; }
    public required long ChatId { get; set; }
    public required string ChatName { get; set; }
    public required ChatType ChatType { get; set; }
    public required long UserId { get; set; }
    public required long MessageId { get; set; }
    public required string UserName { get; set; }
    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public required MessageType MessageType { get; set; }
    public required string MessageText { get; set; }
    public required DateTime SendedAt { get; set; }
}
