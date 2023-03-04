namespace AutoFindBot.Repositories;

public interface IPaymentRepository : IRepository<Entities.Payment>
{
    Task<Entities.Payment> AddAsync(Entities.Payment newPayment);
}