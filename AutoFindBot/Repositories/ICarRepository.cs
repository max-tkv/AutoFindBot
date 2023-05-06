using System.Linq.Expressions;
using AutoFindBot.Entities;

namespace AutoFindBot.Repositories;

public interface ICarRepository
{
    Task<Entities.Car> AddAsync(Car newCar);
    Task<Entities.Car?> GetByFilterAsync(Expression<Func<Car, bool>> predicate);
}