namespace TBMF.Core;

// Public Properties for EF.
public class CommandStateEntity(Guid id, string name, bool isEnabled)
{
    public Guid Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public bool IsEnabled { get; private set; } = isEnabled;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; }

    internal void Enable()
    {
        IsEnabled = true;
    }

    internal void Disable()
    {
        IsEnabled = false;
    }

    internal void Toggle()
    {
        IsEnabled = !IsEnabled;
    }

    internal void RefreshUpdatedTime()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}