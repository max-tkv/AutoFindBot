using Microsoft.Extensions.Configuration;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Helpers;

namespace AutoFindBot.Services;

public class PaginationService : IPaginationService
{
    private readonly int _pageSize;

    public PaginationService(IConfiguration configuration)
    {
        _pageSize = (int?)configuration.GetValue(typeof(int), "PageSize") ?? 
                    throw new PaginationServiceException("Not found PageSize params to config");
    }
    
    public IPaginationResult<T> GetPage<T>(List<T>? dataSource, AppUser user, Categories category, int page = 1)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        return dataSource.AsPagination(page, _pageSize, category);
    }
}