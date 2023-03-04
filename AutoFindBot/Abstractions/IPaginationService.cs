using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface IPaginationService
{
    IPaginationResult<T> GetPage<T>(List<T>? dataSource, AppUser user, Categories category, int page = 1);
}