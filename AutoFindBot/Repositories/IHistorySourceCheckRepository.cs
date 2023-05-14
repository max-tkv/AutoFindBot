using System.Linq.Expressions;
using AutoFindBot.Lookups;

namespace AutoFindBot.Repositories;

public interface ISourceCheckRepository
{
    Task<Entities.SourceCheck> AddAsync(
        Entities.SourceCheck newSourceCheck, 
        CancellationToken stoppingToken = default);

    Task<Entities.SourceCheck?> GetLastByFilterAsync(
        Expression<Func<Entities.SourceCheck, bool>> predicate, 
        CancellationToken stoppingToken = default);

    Task<bool> ExistsAsync(
        Entities.UserFilter filter, 
        SourceType sourceType, 
        CancellationToken stoppingToken = default);

    Task<bool> UpdateDateTimeAsync(
        Entities.UserFilter filter, 
        SourceType sourceType, 
        CancellationToken stoppingToken = default);
}