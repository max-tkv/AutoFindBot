using AutoFindBot.Models.KeyAutoProbeg;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IKeyAutoProbegHttpApiClient
{
    Task<List<KeyAutoProbegResult>> GetAutoByFilterAsync(KeyAutoProbegFilter filter);
}