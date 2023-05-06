using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface IActionService
{
    Task<Entities.Action> GetLastByUserAsync(AppUser appUser);
    
    Task AddAsync(Entities.Action newAction);
}