namespace TBMF.Core;

public record TopUserInfoDto
{
    public required string UserName { get; set; }
    public required string UserFirstName { get; set; }
    public required string UserLastName { get; set; }
    public required int Count { get; set; }
}