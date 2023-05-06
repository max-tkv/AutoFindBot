using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using AutoFindBot.Abstractions;
using AutoFindBot.Repositories;
using AutoFindBot.Storage.Repositories;

namespace AutoFindBot.Storage;

public class UnitOfWork : IUnitOfWork
{
    public IAppUserRepository Users { get; }
    public IActionRepository Actions { get; }
    public IPaymentRepository Payments { get; }
    public ICarRepository Cars { get; }
    public IUserFilterRepository UserFilters { get; }
    public ISourceCheckRepository SourceChecks { get; }
    
    private readonly AppDbContext _appDbContext;
    private bool _isDisposed;
    private readonly SemaphoreSlim _semaphore = new(1);
    
    public UnitOfWork([NotNull] AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;

        Users = new AppUserRepository(appDbContext);
        Actions = new ActionRepository(appDbContext);
        Payments = new PaymentRepository(appDbContext);
        Cars = new CarRepository(appDbContext);
        UserFilters = new UserFilterRepository(appDbContext);
        SourceChecks = new SourceCheckRepository(appDbContext);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _appDbContext.Dispose();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            _appDbContext.ChangeTracker.DetectChanges();
            return await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task DetachAllTrackingEntitiesAsync()
    {
        foreach (var entityEntry in _appDbContext.ChangeTracker.Entries())
        {
            _appDbContext.Attach(entityEntry.Entity).State = EntityState.Detached;
        }
        return Task.CompletedTask;
    }
}