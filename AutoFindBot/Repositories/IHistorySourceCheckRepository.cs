using System.Linq.Expressions;
using AutoFindBot.Entities;

namespace AutoFindBot.Repositories;

public interface ISourceCheckRepository
{
    Task<Entities.SourceCheck> AddAsync(Entities.SourceCheck newSourceCheck);

    Task<Entities.SourceCheck?> GetLastByFilterAsync(Expression<Func<Entities.SourceCheck, bool>> predicate);

    Task<bool> ExistsAsync(UserFilter filter, Source source);

    Task<bool> UpdateDateTimeAsync(UserFilter filter, Source source);
}