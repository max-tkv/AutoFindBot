using AutoFindBot.Abstractions;
using AutoFindBot.Entities;

namespace AutoFindBot.Services;

public class SourceCheckService : ISourceCheckService
{
    private readonly IUnitOfWork _unitOfWork;

    public SourceCheckService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<long> AddSourceAsync(UserFilter filter, Source source)
    {
        var entity = await _unitOfWork.SourceChecks.AddAsync(new SourceCheck()
        {
            Source = source,
            UserFilterId = filter.Id
        });
        await _unitOfWork.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> ExistsAsync(UserFilter filter, Source source)
    {
        return await _unitOfWork.SourceChecks.GetLastByFilterAsync(x => 
                    x.UserFilterId == filter.Id &&
                    x.Source == source) switch
            {
                null => false,
                _ => true
            };
    }
    
    public async Task<bool> ExistsAsync(UserFilter filter)
    {
        return await _unitOfWork.SourceChecks.GetLastByFilterAsync(x => 
                x.UserFilterId == filter.Id) switch
            {
                null => false,
                _ => true
            };
    }
}