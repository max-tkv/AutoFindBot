using AutoFindBot.Entities;
using AutoFindBot.Repositories;

namespace AutoFindBot.Storage.Repositories;

public class PaymentRepository : Repository<Entities.Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Payment> AddAsync(Payment newPayment)
    {
        var result = await DbSet.AddAsync(newPayment);
        return result.Entity;
    }
}