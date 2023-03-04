using System.Collections;
using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface IPaginationResult : IEnumerable
{

    int PageNumber { get; }
    int PageSize { get; }
    int TotalItems { get; }
    int TotalPages { get; }
    Categories Category { get; }
    int FirstItem { get; }
    int LastItem { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}

public interface IPaginationResult<T> : IPaginationResult, IEnumerable<T>
{
}