using System.Linq.Expressions;
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
    
    public async Task<Entities.Car> AddAsync(
        Entities.Car newCar, 
        CancellationToken stoppingToken = default)
    {
        var result = await _context.Cars.AddAsync(newCar, stoppingToken);
        await CommitAsync(stoppingToken);
        return result.Entity;
    }

    public async Task<Entities.Car?> GetByFilterAsync(
        Expression<Func<Entities.Car, bool>> predicate, 
        CancellationToken stoppingToken = default) =>
        await _context.Cars.AsNoTracking()
            .Where(predicate)
            .FirstOrDefaultAsync(cancellationToken: stoppingToken);
    
    private async Task CommitAsync(CancellationToken cancellationToken = default) =>
        await _context.SaveChangesAsync(cancellationToken);
}