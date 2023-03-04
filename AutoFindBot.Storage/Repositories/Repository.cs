using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using AutoFindBot.Repositories;

namespace AutoFindBot.Storage.Repositories
{
	public abstract class Repository<TEntity> : IRepository<TEntity>
		where TEntity : class, new()
	{
		protected AppDbContext Context { get; }

		protected DbSet<TEntity> DbSet { get; }

		protected Repository([NotNull] AppDbContext context)
		{
			Context = context;
			DbSet = context.Set<TEntity>();
		}

		public ValueTask<TEntity> GetAsync(object id) => DbSet.FindAsync(id);

		public void Add(TEntity entity) => Context.Attach(entity).State = EntityState.Added;

		public void Delete(TEntity entity) => Context.Attach(entity).State = EntityState.Deleted;

		public void Update(TEntity entity) => Context.Attach(entity).State = EntityState.Modified;

		public void AttachRange(TEntity[] entities) => Context.AttachRange(entities);
	}
}