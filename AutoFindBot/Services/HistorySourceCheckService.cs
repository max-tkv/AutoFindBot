using AutoFindBot.Abstractions;
using AutoFindBot.Entities;

namespace AutoFindBot.Services;

public class HistorySourceCheckService : IHistorySourceCheckService
{
    private readonly IUnitOfWork _unitOfWork;

    public HistorySourceCheckService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<long> AddErrorAsync(UserFilter filter, Source source, string error)
    {
        var entity = await _unitOfWork.HistorySourceChecks.AddAsync(new HistorySourceCheck()
        {
            Success = false,
            Source = source,
            Data = error,
            UserFilterId = filter.Id
        });
        await _unitOfWork.SaveChangesAsync();

        return entity.Id;
    }
    
    public async Task<long> AddSuccessAsync(UserFilter filter, Source source)
    {
        var entity = await _unitOfWork.HistorySourceChecks.AddAsync(new HistorySourceCheck()
        {
            Success = true,
            Source = source,
            UserFilterId = filter.Id
        });
        await _unitOfWork.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> ExistsSuccessBySourceAsync(UserFilter filter, Source source)
    {
        return await _unitOfWork.HistorySourceChecks.GetLastByFilterAsync(x => 
                    x.UserFilterId == filter.Id && 
                    x.Success == true && 
                    x.Source == source) switch
            {
                null => false,
                _ => true
            };
    }
}