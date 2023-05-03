namespace AutoFindBot.Exceptions;

public class NotActiveSourceException : Exception
{
    public NotActiveSourceException(string message) : base(message)
    {
    }

    public static void ThrowIfNotActive(string httpApiClientName, bool optionsActive)
    {
        if (optionsActive == false)
        {
            Throw($"{httpApiClientName} отключен.");
        }
    }
    
    private static void Throw(string message) =>
        throw new NotActiveSourceException(message);
}