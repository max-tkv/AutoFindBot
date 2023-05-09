using AutoFindBot.Models.AutoRu;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IAutoRuHttpApiClient
{
    Task<AutoRuResult> GetAllNewAutoAsync();
}