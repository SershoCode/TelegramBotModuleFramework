using Microsoft.EntityFrameworkCore;

namespace TBMF.Core;

internal class CommandStateEntityRepository : IRepository<CommandStateEntity>
{
    private readonly TbmfDbContext _dbContext;

    internal CommandStateEntityRepository()
    {
        _dbContext = new TbmfDbContext();
    }

    internal async Task AddMissingAsync(IEnumerable<Type> userTypes)
    {
        var allDbStates = await GetListAsync();

        var missingTypes = userTypes.Where(userType => !allDbStates.Select(dbState => dbState.Name).Contains(userType.Name));

        if (missingTypes.Any())
        {
            await _dbContext.CommandStates.AddRangeAsync(missingTypes.Select(missingCommand => new CommandStateEntity(Guid.NewGuid(), missingCommand.Name, true)));
        }

        await _dbContext.SaveChangesAsync();
    }

    internal async Task<CommandStateEntity> FindByNameAsync(string name)
    {
        var commandState = await _dbContext.CommandStates.FirstOrDefaultAsync(commandState => commandState.Name == name);

        return commandState;
    }

    internal async Task<List<CommandStateEntity>> GetListAsync()
    {
        var commandStates = await _dbContext.CommandStates.ToListAsync();

        return commandStates;
    }

    internal async Task DeleteNotFoundAsync(IEnumerable<Type> userTypes)
    {
        var allDbStates = await GetListAsync();

        var missingStates = allDbStates.Where(dbState => !userTypes.Select(userType => userType.Name).Contains(dbState.Name));

        if (missingStates.Any())
        {
            _dbContext.CommandStates.RemoveRange(missingStates);

            await _dbContext.SaveChangesAsync();
        }
    }

    internal async Task<bool> IsTypeEnabled(Type type)
    {
        var commandState = await FindByNameAsync(type.Name);

        return commandState.IsEnabled;
    }

    internal async Task<bool> IsTypeEnabled(string typeName)
    {
        var commandState = await FindByNameAsync(typeName);

        return commandState.IsEnabled;
    }

    internal async Task ToggleByNameAsync(string typeName)
    {
        var commandState = await FindByNameAsync(typeName);

        commandState.Toggle();

        await _dbContext.SaveChangesAsync();
    }

    internal async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}