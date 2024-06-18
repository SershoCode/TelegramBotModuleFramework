namespace TBMF.Core;

internal static class CommandStateManager
{
    internal static async Task<bool> IsCommandEnabled(Type type)
    {
        var repo = new CommandStateEntityRepository();

        return await repo.IsTypeEnabled(type);
    }
}