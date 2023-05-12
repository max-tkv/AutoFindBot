using AutoFindBot.Models.Drom;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IDromHttpApiClient
{
    Task<List<DromResult>> GetAllNewAutoAsync();
}