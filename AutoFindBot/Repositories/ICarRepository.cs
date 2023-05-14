using System.Linq.Expressions;

namespace AutoFindBot.Repositories;

public interface ICarRepository
{
    Task<Entities.Car> AddAsync(
        Entities.Car newCar, 
        CancellationToken stoppingToken = default);
    
    Task<Entities.Car?> GetByFilterAsync(
        Expression<Func<Entities.Car, bool>> predicate, 
        CancellationToken stoppingToken = default);
}