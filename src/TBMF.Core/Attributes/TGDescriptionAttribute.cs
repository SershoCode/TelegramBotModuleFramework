namespace TBMF.Core;

[AttributeUsage(AttributeTargets.Class)]
public class TgDescriptionAttribute(string commandDescription) : Attribute
{
    public string CommandDescription { get; set; } = commandDescription;
}