using System.Linq.Expressions;
using AutoFindBot.Entities;
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

    public async Task<SourceCheck> AddAsync(SourceCheck newSourceCheck)
    {
        var result = await _context.SourceChecks.AddAsync(newSourceCheck);
        await CommitAsync();
        return result.Entity;
    }

    public async Task<SourceCheck?> GetLastByFilterAsync(Expression<Func<SourceCheck, bool>> predicate) =>
        await _context.SourceChecks.AsNoTracking()
            .Where(predicate)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<bool> ExistsAsync(UserFilter filter, Source source)
    {
        return await GetLastByFilterAsync(x => 
                x.UserFilterId == filter.Id &&
                x.Source == source) switch
            {
                null => false,
                _ => true
            };
    }

    public async Task<bool> UpdateDateTimeAsync(UserFilter filter, Source source)
    {
        var sourceCheck = await _context.SourceChecks.SingleOrDefaultAsync(x => x.Source == source && x.UserFilterId == filter.Id);
        if (sourceCheck != null)
        {
            sourceCheck.UpdatedDateTime = DateTime.Now;
            await CommitAsync();
            return true;
        }

        return false;
    }
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}