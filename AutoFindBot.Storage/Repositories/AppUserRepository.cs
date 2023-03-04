using Microsoft.EntityFrameworkCore;
using AutoFindBot.Repositories;

namespace AutoFindBot.Storage.Repositories;

public class AppUserRepository : Repository<Entities.AppUser>, IAppUserRepository
{
    public AppUserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Entities.AppUser> AddAsync(Entities.AppUser newUser)
    {
        var userEntity = await DbSet.AddAsync(newUser);
        return userEntity.Entity;
    }
    
    public async Task<Entities.AppUser?> GetByChatIdAsync(long chatId)
    {
        return await DbSet.AsNoTracking().Where(x => x.ChatId == chatId)
            .SingleOrDefaultAsync();
    }
}