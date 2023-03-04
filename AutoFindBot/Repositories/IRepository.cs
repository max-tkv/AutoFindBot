using System.Diagnostics.CodeAnalysis;

namespace AutoFindBot.Repositories;

public interface IRepository<TEntity> where TEntity : class, new()
{
    ValueTask<TEntity> GetAsync(object id);
    void Add([NotNull]TEntity entity);
    void Delete([NotNull]TEntity entity);
    void Update([NotNull]TEntity entity);
    void AttachRange(TEntity[] entities);
}