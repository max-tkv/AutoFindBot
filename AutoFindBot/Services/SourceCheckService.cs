using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;

namespace AutoFindBot.Services;

public class SourceCheckService : ISourceCheckService
{
    private readonly ISourceCheckRepository _sourceCheckRepository;

    public SourceCheckService(ISourceCheckRepository sourceCheckRepository)
    {
        _sourceCheckRepository = sourceCheckRepository;
    }

    public async Task<long> AddSourceAsync(UserFilter filter, Source source)
    {
        var entity = await _sourceCheckRepository.AddAsync(new SourceCheck()
        {
            Source = source,
            UserFilterId = filter.Id
        });

        return entity.Id;
    }

    public async Task<bool> ExistsAsync(UserFilter filter, Source source)
    {
        return await _sourceCheckRepository.GetLastByFilterAsync(x => 
                    x.UserFilterId == filter.Id &&
                    x.Source == source) switch
            {
                null => false,
                _ => true
            };
    }
    
    public async Task<bool> ExistsAsync(UserFilter filter)
    {
        return await _sourceCheckRepository.GetLastByFilterAsync(x => 
                x.UserFilterId == filter.Id) switch
            {
                null => false,
                _ => true
            };
    }
}