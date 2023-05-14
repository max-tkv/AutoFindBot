using AutoFindBot.Models.Youla;

namespace AutoFindBot.Abstractions.HttpClients;

public interface IYoulaHttpApiClient
{
    Task<YoulaResult> GetAllNewAutoAsync(CancellationToken stoppingToken = default);
}