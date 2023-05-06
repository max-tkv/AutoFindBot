using AutoFindBot.Entities;

namespace AutoFindBot.Repositories;

public interface IAppUserRepository
{
    Task<Entities.AppUser> AddAsync(Entities.AppUser newUser);
    Task ConfirmAsync(long userId);
    Task<Entities.AppUser?> GetByChatIdAsync(long chatId);
    Task<List<Entities.AppUser>> GetAllAsync();
    Task UpdateAsync(AppUser user);
}