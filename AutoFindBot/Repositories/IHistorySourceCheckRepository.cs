using System.Linq.Expressions;

namespace AutoFindBot.Repositories;

public interface IHistorySourceCheckRepository : IRepository<Entities.HistorySourceCheck>
{
    Task<Entities.HistorySourceCheck> AddAsync(Entities.HistorySourceCheck newHistorySourceCheck);

    Task<Entities.HistorySourceCheck?> GetLastByFilterAsync(Expression<Func<Entities.HistorySourceCheck, bool>> predicate);
}