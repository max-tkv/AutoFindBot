using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface ISourceCheckService
{
    Task<long> AddSourceAsync(UserFilter filter, Source source);

    Task<bool> ExistsAsync(UserFilter filter, Source source);
}