using System.Collections;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;

namespace AutoFindBot.Models.Pagination;

public class PaginationResult<T> : IPaginationResult<T>
{
    public const int DefaultPageSize = 20;
    private IList<T> results;
    private int totalItems;
    public int PageSize { get; private set; }
    public IQueryable<T> DataSource { get; protected set; }
    public int PageNumber { get; private set; }
    
    public PaginationResult(IQueryable<T> dataSource, int pageNumber, int pageSize, Categories category)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        DataSource = dataSource;
        Category = category;
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        TryExecuteQuery();

        foreach (var item in results)
        {
            yield return item;
        }
    }
    
    protected void TryExecuteQuery()
    {
        if (results != null)
            return;

        totalItems = DataSource.Count();
        results = ExecuteQuery();
    }
    
    protected virtual IList<T> ExecuteQuery()
    {
        int numberToSkip = (PageNumber - 1) * PageSize;
        return DataSource.Skip(numberToSkip).Take(PageSize).ToList();
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    public int TotalItems
    {
        get
        {
            TryExecuteQuery();
            return totalItems;
        }
    }

    public int TotalPages
    {
        get { return (int)Math.Ceiling(((double)TotalItems) / PageSize); }
    }

    public Categories Category { get; }

    public int FirstItem
    {
        get
        {
            TryExecuteQuery();
            return ((PageNumber - 1) * PageSize) + 1;
        }
    }

    public int LastItem
    {
        get { return FirstItem + results.Count - 1; }
    }

    public bool HasPreviousPage
    {
        get { return PageNumber > 1; }
    }

    public bool HasNextPage
    {
        get { return PageNumber < TotalPages; }
    }
}