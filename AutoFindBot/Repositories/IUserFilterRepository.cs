using AutoFindBot.Entities;

namespace AutoFindBot.Repositories;

public interface IUserFilterRepository
{
    Task<UserFilter> AddAsync(UserFilter newUserFilter);
    Task<UserFilter> DeleteAsync(UserFilter userFilterForDelete);
    Task<List<UserFilter>> GetByUserAsync(AppUser user);
}