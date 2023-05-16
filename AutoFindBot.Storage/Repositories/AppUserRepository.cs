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

    public async Task<Entities.AppUser> AddAsync(
        Entities.AppUser newUser, 
        CancellationToken stoppingToken = default)
    {
        var userEntity = await _context.Users.AddAsync(newUser, stoppingToken);
        await CommitAsync(stoppingToken);
        return userEntity.Entity;
    }
    
    public async Task ConfirmAsync(
        long userId, 
        CancellationToken stoppingToken = default)
    {
        var user = await _context.Users
            .SingleAsync(x => x.Id == userId, cancellationToken: stoppingToken);
        user.Confirm = true;
        user.UpdatedDateTime = DateTime.Now;

        await CommitAsync(stoppingToken);
    }
    
    public async Task<Entities.AppUser?> GetByChatIdAsync(
        long chatId, 
        CancellationToken stoppingToken = default)
    {
        return await _context.Users.AsNoTracking()
            .Where(x => x.ChatId == chatId)
            .SingleOrDefaultAsync(cancellationToken: stoppingToken);
    }

    public async Task<List<Entities.AppUser>> GetAllAsync(CancellationToken stoppingToken = default)
    {
        return await _context.Users.AsNoTracking()
            .ToListAsync(cancellationToken: stoppingToken);
    }

    public async Task UpdateAsync(
        Entities.AppUser user, 
        CancellationToken stoppingToken = default)
    {
        _context.Users.Update(user);
        await CommitAsync(stoppingToken);
    }

    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}