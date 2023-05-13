using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Lookups;
using AutoFindBot.Repositories;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class CarService : ICarService
{
    private readonly ILogger<CarService> _logger;
    private readonly ICarRepository _carRepository;

    public CarService(
        ILogger<CarService> logger, 
        ICarRepository carRepository)
    {
        _logger = logger;
        _carRepository = carRepository;
    }
    
    public async Task<bool> CheckExistNewCarAndSaveAsync(
        Car newCar, 
        UserFilter userFilter,
        SourceType sourceType)
    {
        var car = await _carRepository
            .GetByFilterAsync(x => x.OriginId == newCar.OriginId
                                   && x.UserFilterId == userFilter.Id
                                   && x.SourceType == sourceType);
        if (car == null)
        {
            newCar.UserFilterId = userFilter.Id;
            await _carRepository.AddAsync(newCar);

            return true;
        }

        return false;
    }
}