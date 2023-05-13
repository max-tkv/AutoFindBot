using System.Linq.Expressions;
using AutoFindBot.Lookups;

namespace AutoFindBot.Repositories;

public interface ISourceCheckRepository
{
    Task<Entities.SourceCheck> AddAsync(Entities.SourceCheck newSourceCheck);

    Task<Entities.SourceCheck?> GetLastByFilterAsync(Expression<Func<Entities.SourceCheck, bool>> predicate);

    Task<bool> ExistsAsync(Entities.UserFilter filter, SourceType sourceType);

    Task<bool> UpdateDateTimeAsync(Entities.UserFilter filter, SourceType sourceType);
}