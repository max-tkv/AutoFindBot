using AutoFindBot.Models.Avito;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IAvitoHttpApiClient : IBaseIntegrationHttpApiClient
{
    Task<List<AvitoResult>> GetAutoByFilterAsync(AvitoFilter filter);
}