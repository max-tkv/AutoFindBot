using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface IUserFilterService
{
    Task<List<UserFilter>> GetByUserAsync(AppUser appUser);
}