using AutoFindBot.Entities;

namespace AutoFindBot.Repositories;

public interface IUserFilterRepository
{
    Task<UserFilter> AddAsync(
        UserFilter newUserFilter, 
        CancellationToken stoppingToken = default);
    
    Task<UserFilter> DeleteAsync(
        UserFilter userFilterForDelete, 
        CancellationToken stoppingToken = default);
    
    Task<List<UserFilter>> GetByUserAsync(
        AppUser user, 
        CancellationToken stoppingToken = default);
}