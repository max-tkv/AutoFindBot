namespace AutoFindBot.Exceptions;

public class PaginationServiceException : Exception
{
    public PaginationServiceException(string message) : base(message)
    {
    }
}