using AutoFindBot.Models.AutoRu;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IAutoRuHttpApiClient
{
    Task<AutoRuResult> GetAutoByFilterAsync(AutoRuFilter filter);
    Task<AutoRuResult> GetAllNewAutoAsync();
}