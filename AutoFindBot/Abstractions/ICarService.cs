using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface ICarService
{
    Task<List<Car>> GetNewCarsAndSaveAsync(List<Car> cars, UserFilter userFilter, long historySourceCheckId);
    Task AddCarRangeAsync(List<Car> cars);
}