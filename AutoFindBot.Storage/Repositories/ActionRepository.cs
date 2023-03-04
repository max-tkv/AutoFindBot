using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Action = AutoFindBot.Entities.Action;

namespace AutoFindBot.Storage.Repositories;

public class ActionRepository : Repository<Entities.Action>, IActionRepository
{
    public ActionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Entities.Action> AddAsync(Entities.Action newAction)
    {
        var result = await DbSet.AddAsync(newAction);
        return result.Entity;
    }

    public async Task<Entities.Action> GetLastAsync(long id) =>
        await DbSet.AsNoTracking().Where(x => x.Id == id)
            .OrderByDescending(x => x.CreatedAt)
            .SingleOrDefaultAsync();

    public async Task<Entities.Action> GetLastByUserAsync(AppUser user) =>
        await DbSet.AsNoTracking().Where(x => x.User == user)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Entities.Action> GetLastByUserAsync(AppUser user, string command) =>
        await DbSet.AsNoTracking().Where(x => x.User == user)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Entities.Action> GetLastByFilterAsync(Expression<Func<Action,bool>> predicate) =>
        await DbSet.AsNoTracking()
            .Where(predicate)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<int> GetNumberOfRequestsByUserAsync(AppUser user) =>
        await DbSet.AsNoTracking()
            .CountAsync(x => x.User == user);
}