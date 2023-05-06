using System.Linq.Expressions;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class CarRepository : Repository<Entities.Car>, ICarRepository
{
    public CarRepository(AppDbContext context) : base(context)
    {
    }
    
    public async Task<Car> AddAsync(Car newCar)
    {
        var result = await DbSet.AddAsync(newCar);
        return result.Entity;
    }

    public async Task<Entities.Car?> GetByFilterAsync(Expression<Func<Car, bool>> predicate) =>
        await DbSet.AsNoTracking()
            .Where(predicate)
            .FirstOrDefaultAsync();
}