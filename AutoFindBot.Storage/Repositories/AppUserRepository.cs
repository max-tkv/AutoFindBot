using AutoFindBot.Entities;
using Microsoft.EntityFrameworkCore;
using AutoFindBot.Repositories;

namespace AutoFindBot.Storage.Repositories;

public class AppUserRepository : IAppUserRepository
{
    private readonly AppDbContext _context;

    public AppUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Entities.AppUser> AddAsync(Entities.AppUser newUser)
    {
        var userEntity = await _context.Users.AddAsync(newUser);
        return userEntity.Entity;
    }
    
    public async Task ConfirmAsync(long userId)
    {
        var user = await _context.Users.SingleAsync(x => x.Id == userId);
        user.Confirm = true;
        user.UpdatedDateTime = DateTime.Now;

        await CommitAsync();
    }
    
    public async Task<Entities.AppUser?> GetByChatIdAsync(long chatId)
    {
        return await _context.Users.AsNoTracking().Where(x => x.ChatId == chatId)
            .SingleOrDefaultAsync();
    }

    public async Task<List<AppUser>> GetAllAsync()
    {
        return await _context.Users.AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(AppUser user)
    {
        _context.Users.Update(user);
        await CommitAsync();
    }

    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}