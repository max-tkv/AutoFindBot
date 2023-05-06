using System.Linq.Expressions;
using AutoFindBot.Entities;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class CarRepository : ICarRepository
{
    private readonly AppDbContext _context;

    public CarRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Car> AddAsync(Car newCar)
    {
        var result = await _context.Cars.AddAsync(newCar);
        await CommitAsync();
        return result.Entity;
    }

    public async Task<Entities.Car?> GetByFilterAsync(Expression<Func<Car, bool>> predicate) =>
        await _context.Cars.AsNoTracking()
            .Where(predicate)
            .FirstOrDefaultAsync();
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}