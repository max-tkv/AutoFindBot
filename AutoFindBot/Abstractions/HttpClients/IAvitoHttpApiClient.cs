using AutoFindBot.Models.Avito;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IAvitoHttpApiClient
{
    Task<List<AvitoResult>> GetAllNewAutoAsync();
    
    Task<List<AvitoResult>> GetAutoByFilterAsync(AvitoFilter filter);
}