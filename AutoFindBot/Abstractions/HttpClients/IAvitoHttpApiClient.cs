using AutoFindBot.Models.Avito;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IAvitoHttpApiClient
{
    Task<List<AvitoResult>> GetAllNewAutoAsync(CancellationToken stoppingToken = default);
    
    Task<List<AvitoResult>> GetAutoByFilterAsync(AvitoFilter filter, CancellationToken stoppingToken = default);
}