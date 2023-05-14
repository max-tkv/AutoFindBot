using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface IActionService
{
    Task<Entities.Action?> GetLastByUserAsync(
        AppUser appUser, 
        CancellationToken stoppingToken = default);
    
    Task AddAsync(
        Entities.Action newAction, 
        CancellationToken stoppingToken = default);
}