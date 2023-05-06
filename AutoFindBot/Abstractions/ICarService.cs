using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface ICarService
{
    Task<List<Car>> CheckExistNewCarsAndSaveAsync(List<Car> cars, UserFilter userFilter);
    Task AddCarRangeAsync(List<Car> cars);
}