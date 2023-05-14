using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions;

public interface IAppUserService
{
    Task<Entities.AppUser> GetOrCreateAsync(
        Update update, 
        CancellationToken stoppingToken = default);
    
    Task<Entities.AppUser> GetOrCreateAsync(
        Entities.AppUser newUser, 
        CancellationToken stoppingToken = default);
    
    Entities.AppUser GetByUpdate(Update update);
    
    Task CheckFreeRequestAsync(
        Entities.AppUser user, 
        CancellationToken stoppingToken = default);
    
    Task CheckRequiredSubscriptionsAsync(
        TelegramBotClient botClient, 
        Entities.AppUser user, 
        CancellationToken stoppingToken = default);
    
    Task<List<Entities.AppUser>> GetAllAsync(
        CancellationToken stoppingToken = default);
    
    Task SetConfirmAsync(
        long currentFilterUserId, 
        CancellationToken stoppingToken = default);
}