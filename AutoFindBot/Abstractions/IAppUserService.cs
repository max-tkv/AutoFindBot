using AutoFindBot.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions;

public interface IAppUserService
{
    Task<Entities.AppUser> GetOrCreateAsync(Update update);
    Task<Entities.AppUser> GetOrCreateAsync(Entities.AppUser newUser);
    Entities.AppUser GetByUpdate(Update update);
    Task CheckFreeRequestAsync(AppUser user);
    Task CheckRequiredSubscriptionsAsync(TelegramBotClient botClient, AppUser user);
    Task<List<Entities.AppUser>> GetAllAsync();
    Task SetConfirmAsync(long currentFilterUserId);
}