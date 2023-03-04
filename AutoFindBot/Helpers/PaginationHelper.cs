using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Models.Pagination;

namespace AutoFindBot.Helpers;

public static class PaginationHelper
{
    public static IPaginationResult<T> AsPagination<T>(this IEnumerable<T> source, int pageNumber, Categories category)
    {
        return source.AsPagination(pageNumber, PaginationResult<T>.DefaultPageSize, category);
    }
    
    public static IPaginationResult<T> AsPagination<T>(this IEnumerable<T> source, int pageNumber, int pageSize, Categories category)
    {
        if(pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException("pageNumber", "The page number should be greater than or equal to 1.");
        }

        return new PaginationResult<T>(source.AsQueryable(), pageNumber, pageSize, category);
    }
}