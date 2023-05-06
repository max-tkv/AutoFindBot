using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Action = AutoFindBot.Entities.Action;

namespace AutoFindBot.Storage.Repositories;

public class ActionRepository : IActionRepository
{
    private readonly AppDbContext _context;

    public ActionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.Action> AddAsync(Entities.Action newAction)
    {
        var result = await _context.Actions.AddAsync(newAction);
        await CommitAsync();
        return result.Entity;
    }

    public async Task<Entities.Action> GetLastAsync(long id) =>
        await _context.Actions.AsNoTracking().Where(x => x.Id == id)
            .OrderByDescending(x => x.CreatedAt)
            .SingleOrDefaultAsync();

    public async Task<Entities.Action> GetLastByUserAsync(AppUser user) =>
        await _context.Actions.AsNoTracking().Where(x => x.User == user)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Entities.Action> GetLastByUserAsync(AppUser user, string command) =>
        await _context.Actions.AsNoTracking().Where(x => x.User == user)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<Entities.Action> GetLastByFilterAsync(Expression<Func<Action, bool>> predicate) =>
        await _context.Actions.AsNoTracking()
            .Where(predicate)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<int> GetNumberOfRequestsByUserAsync(AppUser user) =>
        await _context.Actions.AsNoTracking()
            .CountAsync(x => x.User == user);
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}