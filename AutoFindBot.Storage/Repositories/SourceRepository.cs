using AutoFindBot.Entities;
using AutoFindBot.Lookups;
using AutoFindBot.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AutoFindBot.Storage.Repositories;

public class SourceRepository : ISourceRepository
{
    private readonly AppDbContext _context;

    public SourceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Source> GetByTypeAsync(SourceType sourceType) =>
        await _context.Sources.AsNoTracking()
            .SingleAsync(x => x.Id == (long)sourceType);
}