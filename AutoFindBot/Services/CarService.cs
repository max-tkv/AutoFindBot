using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class CarService : ICarService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CarService(
        ILogger<PaymentService> logger, 
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
            if (car == null)
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
}