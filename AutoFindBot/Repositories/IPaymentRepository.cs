namespace AutoFindBot.Repositories;

public interface IPaymentRepository
{
    Task<Entities.Payment> AddAsync(Entities.Payment newPayment);
}