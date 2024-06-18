namespace TBMF.Core;

public record ToggleCommandCallBackDto(string commandName) : ICallbackDto
{
    public string NameOfDto { get; } = nameof(ToggleCommandCallBackDto);
    public string CommandName { get; } = commandName;

    public override string ToString() => $"{NameOfDto}:{CommandName}";
};