using AutoFindBot.Entities;
using AutoFindBot.Repositories;

namespace AutoFindBot.Storage.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payment> AddAsync(Payment newPayment)
    {
        var result = await _context.Payments.AddAsync(newPayment);
        await CommitAsync();
        return result.Entity;
    }
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}