using System.Linq.Expressions;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class SourceCheckRepository : Repository<Entities.SourceCheck>, ISourceCheckRepository
{
    public SourceCheckRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<SourceCheck> AddAsync(SourceCheck newSourceCheck)
    {
        var result = await DbSet.AddAsync(newSourceCheck);
        return result.Entity;
    }

    public async Task<SourceCheck?> GetLastByFilterAsync(Expression<Func<SourceCheck, bool>> predicate) =>
        await DbSet.AsNoTracking()
            .Where(predicate)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<bool> UpdateDateTimeAsync(UserFilter filter, Source source)
    {
        var sourceCheck = await DbSet.SingleOrDefaultAsync(x => x.Source == source && x.UserFilterId == filter.Id);
        if (sourceCheck != null)
        {
            sourceCheck.UpdatedDateTime = DateTime.Now;
            return true;
        }

        return false;
    }
}