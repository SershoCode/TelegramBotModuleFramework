namespace TBMF.Core;

[Flags]
public enum TextTriggerType
{
    StartsWith = 1,
    EndsWith = 2,
    Contains = 4,
    Equals = 8
}