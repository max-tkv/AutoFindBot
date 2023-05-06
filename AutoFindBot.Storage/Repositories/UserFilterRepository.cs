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
    
    public async Task<UserFilter> AddAsync(UserFilter newUserFilter)
    {
        var result = await _context.UserFilters.AddAsync(newUserFilter);
        await CommitAsync();
        return result.Entity;
    }
    
    public async Task<UserFilter> DeleteAsync(UserFilter userFilterForDelete)
    {
        var userFilter = await _context.UserFilters.Where(x => x.Id == userFilterForDelete.Id).SingleAsync();
        var result = await _context.UserFilters.AddAsync(userFilter);
        await CommitAsync();
        return result.Entity;
    }
    
    public async Task<List<UserFilter>> GetByUserAsync(AppUser user)
    {
        var result = await _context.UserFilters.Where(x => x.UserId == user.Id).ToListAsync();
        return result;
    }
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}