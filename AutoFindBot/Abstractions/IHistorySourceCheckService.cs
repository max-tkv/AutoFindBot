using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface IHistorySourceCheckService
{
    Task<long> AddErrorAsync(UserFilter filter, Source source, string error);

    Task<long> AddSuccessAsync(UserFilter filter, Source source);

    Task<bool> ExistsSuccessBySourceAsync(UserFilter filter, Source source);
}