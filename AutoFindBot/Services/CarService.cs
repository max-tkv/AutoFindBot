using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Services;

public class CarService : ICarService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CarService(
        ILogger<PaymentService> logger, 
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    
    public async Task<List<CarInfo>> GetNewCarsAndSaveAsync(List<CarInfo> carsInput, AppUser user, UserFilter userFilter)
    {
        var newCars = new List<CarInfo>();
        foreach (var carData in carsInput)
        {
            var carInfo = _mapper.Map<CarInfo, Car>(carData);
            var car = await _unitOfWork.Cars
                .GetByFilterAsync(x => x.OriginId == carInfo.OriginId 
                                       && x.UserFilterId == userFilter.Id 
                                       && x.UserId == user.Id);
            if (car == null)
            {
                newCars.Add(carData);
                
                carInfo.UserId = user.Id;
                carInfo.UserFilterId = userFilter.Id;
                await _unitOfWork.Cars.AddAsync(carInfo);
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