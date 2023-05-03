using AutoFindBot.Repositories;

namespace AutoFindBot.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IAppUserRepository Users { get; }
    
    IActionRepository Actions { get; }
    
    IPaymentRepository Payments { get; }
    
    ICarRepository Cars { get; }
    
    IUserFilterRepository UserFilters { get; }
    
    public IHistorySourceCheckRepository HistorySourceChecks { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task DetachAllTrackingEntitiesAsync();
}