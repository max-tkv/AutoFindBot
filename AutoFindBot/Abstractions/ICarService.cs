using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface ICarService
{
    Task<List<Car>> CheckExistNewCarsAndSaveAsync(List<Car> cars, UserFilter userFilter, Source source);
    Task AddCarRangeAsync(List<Car> cars);
}