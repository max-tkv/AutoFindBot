using System.Linq.Expressions;
using AutoFindBot.Entities;
using AutoFindBot.Lookups;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class SourceCheckRepository : ISourceCheckRepository
{
    private readonly AppDbContext _context;

    public SourceCheckRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SourceCheck> AddAsync(
        SourceCheck newSourceCheck, 
        CancellationToken stoppingToken = default)
    {
        var result = await _context.SourceChecks.AddAsync(newSourceCheck, stoppingToken);
        await CommitAsync(stoppingToken);
        return result.Entity;
    }

    public async Task<SourceCheck?> GetLastByFilterAsync(
        Expression<Func<SourceCheck, bool>> predicate, 
        CancellationToken stoppingToken = default) =>
        await _context.SourceChecks.AsNoTracking()
            .Where(predicate)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken: stoppingToken);

    public async Task<bool> ExistsAsync(
        UserFilter filter, 
        SourceType sourceType, 
        CancellationToken stoppingToken = default)
    {
        return await GetLastByFilterAsync(x => 
                x.UserFilterId == filter.Id &&
                x.SourceType == sourceType, stoppingToken) switch
            {
                null => false,
                _ => true
            };
    }

    public async Task<bool> UpdateDateTimeAsync(
        UserFilter filter, 
        SourceType sourceType, 
        CancellationToken stoppingToken = default)
    {
        var sourceCheck = await _context.SourceChecks
            .SingleOrDefaultAsync(x => x.SourceType == sourceType && 
                                       x.UserFilterId == filter.Id, cancellationToken: stoppingToken);
        if (sourceCheck != null)
        {
            sourceCheck.UpdatedDateTime = DateTime.Now;
            await CommitAsync(stoppingToken);
            return true;
        }

        return false;
    }
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}