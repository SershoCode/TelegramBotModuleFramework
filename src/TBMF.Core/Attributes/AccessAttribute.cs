namespace TBMF.Core;

[AttributeUsage(AttributeTargets.Class)]
public class AccessAttribute(params long[] users) : Attribute
{
    public long[] Users { get; set; } = users;
}