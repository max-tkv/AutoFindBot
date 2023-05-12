namespace AutoFindBot.Abstractions;

public interface ICaptchaSolutionsService
{
    Task SolutionAutoRuAsync(HttpRequestMessage httpRequestMessage);

    Task SolutionKeyAutoProbegAsync(HttpRequestMessage httpRequestMessage);
    
    Task SolutionDromAsync(HttpRequestMessage request);
}