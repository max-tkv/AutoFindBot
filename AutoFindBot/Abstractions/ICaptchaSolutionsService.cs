namespace AutoFindBot.Abstractions;

public interface ICaptchaSolutionsService
{
    Task SolutionAsync(string html);
}