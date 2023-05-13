using AutoFindBot.Entities;
using AutoFindBot.Lookups;

namespace AutoFindBot.Abstractions;

public interface ICarService
{
    Task<bool> CheckExistNewCarAndSaveAsync(Car newCar, UserFilter userFilter, SourceType sourceType);
}