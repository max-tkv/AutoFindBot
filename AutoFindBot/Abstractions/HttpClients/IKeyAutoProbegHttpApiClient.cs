using AutoFindBot.Models.KeyAutoProbeg;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IKeyAutoProbegHttpApiClient : IBaseIntegrationHttpApiClient
{
    Task<List<KeyAutoProbegResult>> GetAutoByFilterAsync(KeyAutoProbegFilter filter);
}