namespace AutoFindBot.Repositories;

public interface IActionRepository
{
    Task<Entities.Action> AddAsync(
        Entities.Action action, 
        CancellationToken stoppingToken = default);

    Task<Entities.Action?> GetLastByUserAsync(
        Entities.AppUser user, 
        CancellationToken stoppingToken = default);

    Task<int> GetNumberOfRequestsByUserAsync(
        Entities.AppUser user, 
        CancellationToken stoppingToken = default);
}