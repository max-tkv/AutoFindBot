using AutoFindBot.Entities;

namespace AutoFindBot.Abstractions;

public interface ICarService
{
    Task<bool> CheckExistNewCarAndSaveAsync(Car newCar, UserFilter userFilter, Source source);
}