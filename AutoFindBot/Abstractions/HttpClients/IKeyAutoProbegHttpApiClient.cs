using AutoFindBot.Models.KeyAutoProbeg;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IKeyAutoProbegHttpApiClient
{
    Task<List<KeyAutoProbegResult>> GetAutoByFilterAsync(KeyAutoProbegFilter filter);
}