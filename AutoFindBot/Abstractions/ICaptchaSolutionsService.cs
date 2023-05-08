namespace AutoFindBot.Abstractions;

public interface ICaptchaSolutionsService
{
    Task SolutionAsync(HttpRequestMessage httpRequestMessage);
}