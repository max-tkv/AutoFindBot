namespace AutoFindBot.Exceptions;

public class NotActiveSourceException : Exception
{
    public NotActiveSourceException(string message) : base(message)
    {
    }

    public static void ThrowIfNotActive(Entities.Source source)
    {
        if (source.Active == false)
        {
            Throw($"{source.Name} отключен.");
        }
    }
    
    private static void Throw(string message) =>
        throw new NotActiveSourceException(message);
}