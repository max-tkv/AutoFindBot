namespace AutoFindBot.Repositories;

public interface IAppUserRepository : IRepository<Entities.AppUser>
{
    Task<Entities.AppUser> AddAsync(Entities.AppUser newUser);
    Task ConfirmAsync(long userId);
    Task<Entities.AppUser?> GetByChatIdAsync(long chatId);
    Task<List<Entities.AppUser>> GetAllAsync();
}