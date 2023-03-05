using AutoFindBot.Entities;

namespace AutoFindBot.Repositories;

public interface ICarRepository
{
    Task<Entities.Car> AddAsync(Car newCar);
    Task<Entities.Car> GetByUserAsync(AppUser user);
    Task<Entities.Car> GetByUserAndSourceAsync(AppUser user, Source source);
}