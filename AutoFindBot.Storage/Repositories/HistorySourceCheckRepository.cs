using System.Linq.Expressions;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class HistorySourceCheckRepository : Repository<Entities.HistorySourceCheck>, IHistorySourceCheckRepository
{
    public HistorySourceCheckRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<HistorySourceCheck> AddAsync(HistorySourceCheck newHistorySourceCheck)
    {
        var result = await DbSet.AddAsync(newHistorySourceCheck);
        return result.Entity;
    }

    public async Task<HistorySourceCheck?> GetLastByFilterAsync(Expression<Func<HistorySourceCheck, bool>> predicate) =>
        await DbSet.AsNoTracking()
            .Where(predicate)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
}