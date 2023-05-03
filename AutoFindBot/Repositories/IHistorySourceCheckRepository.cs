using System.Linq.Expressions;

namespace AutoFindBot.Repositories;

public interface ISourceCheckRepository : IRepository<Entities.SourceCheck>
{
    Task<Entities.SourceCheck> AddAsync(Entities.SourceCheck newSourceCheck);

    Task<Entities.SourceCheck?> GetLastByFilterAsync(Expression<Func<Entities.SourceCheck, bool>> predicate);
}