namespace AutoFindBot.Repositories;

public interface IAppUserRepository
{
    Task<Entities.AppUser> AddAsync(
        Entities.AppUser newUser, 
        CancellationToken stoppingToken = default);
    
    Task ConfirmAsync(
        long userId, 
        CancellationToken stoppingToken = default);
    
    Task<Entities.AppUser?> GetByChatIdAsync(
        long chatId, 
        CancellationToken stoppingToken = default);
    
    Task<List<Entities.AppUser>> GetAllAsync(
        CancellationToken stoppingToken = default);
    
    Task UpdateAsync(Entities.AppUser user, 
        CancellationToken stoppingToken = default);
}