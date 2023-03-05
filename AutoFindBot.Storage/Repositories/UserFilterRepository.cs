using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class UserFilterRepository : Repository<Entities.UserFilter>, IUserFilterRepository
{
    public UserFilterRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<UserFilter> AddAsync(UserFilter newUserFilter)
    {
        var result = await DbSet.AddAsync(newUserFilter);
        return result.Entity;
    }
    
    public async Task<UserFilter> DeleteAsync(UserFilter userFilterForDelete)
    {
        var userFilter = await DbSet.Where(x => x.Id == userFilterForDelete.Id).SingleAsync();
        var result = await DbSet.AddAsync(userFilter);
        return result.Entity;
    }
    
    public async Task<List<UserFilter>> GetByUserAsync(AppUser user)
    {
        var result = await DbSet.Where(x => x.UserId == user.Id).ToListAsync();
        return result;
    }
}