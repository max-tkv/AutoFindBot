using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class UserFilterRepository : IUserFilterRepository
{
    private readonly AppDbContext _context;

    public UserFilterRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserFilter> AddAsync(
        UserFilter newUserFilter, 
        CancellationToken stoppingToken = default)
    {
        var result = await _context.UserFilters.AddAsync(newUserFilter, stoppingToken);
        await CommitAsync(stoppingToken);
        return result.Entity;
    }
    
    public async Task<UserFilter> DeleteAsync(
        UserFilter userFilterForDelete, 
        CancellationToken stoppingToken = default)
    {
        var userFilter = await _context.UserFilters
            .Where(x => x.Id == userFilterForDelete.Id)
            .SingleAsync(cancellationToken: stoppingToken);
        var result = await _context.UserFilters.AddAsync(userFilter, stoppingToken);
        await CommitAsync(stoppingToken);
        return result.Entity;
    }
    
    public async Task<List<UserFilter>> GetByUserAsync(
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        var result = await _context.UserFilters
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken: stoppingToken);
        return result;
    }
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}