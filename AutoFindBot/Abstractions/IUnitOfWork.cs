using AutoFindBot.Repositories;

namespace AutoFindBot.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IAppUserRepository Users { get; }
    IActionRepository Actions { get; }
    IPaymentRepository Payments { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task DetachAllTrackingEntitiesAsync();
}