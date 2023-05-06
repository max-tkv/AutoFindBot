using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class CarService : ICarService
{
    private readonly ILogger<CarService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CarService(
        ILogger<CarService> logger, 
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<Car>> GetNewCarsAndSaveAsync(
        List<Car> carsInput, 
        UserFilter userFilter)
    {
        var newCars = new List<Car>();
        foreach (var carData in carsInput)
        {
            var car = await _unitOfWork.Cars
                .GetByFilterAsync(x => x.OriginId == carData.OriginId 
                                       && x.UserFilterId == userFilter.Id 
                                       && x.UserId == userFilter.UserId);
            if (car == null && CheckFilterYear(userFilter, carData))
            {
                newCars.Add(carData);
                
                carData.UserId = userFilter.UserId;
                carData.UserFilterId = userFilter.Id;
                await _unitOfWork.Cars.AddAsync(carData);
            }
        }

        if (newCars.Any())
        {
            await _unitOfWork.SaveChangesAsync();
        }

        return newCars;
    }

    public async Task AddCarRangeAsync(List<Car> cars)
    {
        foreach (var car in cars)
        {
            await _unitOfWork.Cars.AddAsync(car);
        }
        await _unitOfWork.SaveChangesAsync();
    }
    
    private bool CheckFilterYear(UserFilter userFilter, Car? newCar)
    {
        if (newCar == null)
        {
            _logger.LogInformation($"FilterID: {userFilter.Id}. False");
            return false;
        }
        
        if (userFilter.YearMax == -1 && userFilter.YearMin == -1)
        {
            _logger.LogInformation($"FilterID: {userFilter.Id}. Car Year={newCar.Year}. True");
            return true;
        }

        if (userFilter.YearMax >= newCar.Year && userFilter.YearMin <= newCar.Year)
        {
            _logger.LogInformation($"FilterID: {userFilter.Id}. Car Year={newCar.Year}. True");
            return true;
        }

        _logger.LogInformation($"FilterID: {userFilter.Id}. Car Year={newCar.Year}. False");
        return false;
    }
}