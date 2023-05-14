using AutoFindBot.Lookups;

namespace AutoFindBot.Repositories;

public interface ISourceRepository
{
    Task<Entities.Source> GetByTypeAsync(
        SourceType sourceType, 
        CancellationToken stoppingToken = default);
}