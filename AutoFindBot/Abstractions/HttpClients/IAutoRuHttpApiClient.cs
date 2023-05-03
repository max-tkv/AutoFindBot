using AutoFindBot.Models.AutoRu;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IAutoRuHttpApiClient
{
    Task<AutoRuResult> GetAutoByFilterAsync(AutoRuFilter filter);
}