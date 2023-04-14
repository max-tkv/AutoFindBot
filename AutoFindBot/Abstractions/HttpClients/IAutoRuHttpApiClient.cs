using AutoFindBot.Models.AutoRu;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IAutoRuHttpApiClient
{
    Task<List<AutoRuResult>?> GetAutoByFilterAsync(AutoRuFilter filter);
}