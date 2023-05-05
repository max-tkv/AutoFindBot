using AutoFindBot.Entities;
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
    
    public async Task ConfirmAsync(long userId)
    {
        var user = await DbSet.SingleAsync(x => x.Id == userId);
        user.Confirm = true;
    }
    
    public async Task<Entities.AppUser?> GetByChatIdAsync(long chatId)
    {
        return await DbSet.AsNoTracking().Where(x => x.ChatId == chatId)
            .SingleOrDefaultAsync();
    }

    public async Task<List<AppUser>> GetAllAsync()
    {
        return await DbSet.AsNoTracking().ToListAsync();
    }
}