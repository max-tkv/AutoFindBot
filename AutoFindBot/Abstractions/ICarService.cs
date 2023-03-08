using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;

namespace AutoFindBot.Abstractions;

public interface ICarService
{
    Task<List<Car>> GetNewCarsAndSaveAsync(List<Car> cars, AppUser user, UserFilter userFilter);
    Task AddCarRangeAsync(List<Car> cars);
}