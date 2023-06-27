namespace AutoFindBot.Abstractions;

public interface ICaptchaSolutionsService
{
    Task SolutionAutoRuAsync(
        HttpRequestMessage httpRequestMessage, 
        CancellationToken stoppingToken = default);

    Task SolutionKeyAutoProbegAsync(
        HttpRequestMessage httpRequestMessage, 
        CancellationToken stoppingToken = default);
    
    Task SolutionDromAsync(
        HttpRequestMessage request, 
        CancellationToken stoppingToken = default);

    Task SolutionAvitoAsync(
        HttpRequestMessage request, 
        CancellationToken stoppingToken = default);
}