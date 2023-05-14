using Microsoft.EntityFrameworkCore;
using AutoFindBot.Repositories;

namespace AutoFindBot.Storage.Repositories;

public class ActionRepository : IActionRepository
{
    private readonly AppDbContext _context;

    public ActionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.Action> AddAsync(
        Entities.Action newAction, 
        CancellationToken stoppingToken = default)
    {
        var result = await _context.Actions.AddAsync(newAction, stoppingToken);
        await CommitAsync(stoppingToken);
        return result.Entity;
    }

    public async Task<Entities.Action?> GetLastByUserAsync(
        Entities.AppUser user, 
        CancellationToken stoppingToken = default) =>
        await _context.Actions.AsNoTracking()
            .Where(x => x.User == user)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken: stoppingToken);

    public async Task<int> GetNumberOfRequestsByUserAsync(
        Entities.AppUser user, 
        CancellationToken stoppingToken = default) =>
        await _context.Actions.AsNoTracking()
            .CountAsync(x => x.User == user, cancellationToken: stoppingToken);
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}