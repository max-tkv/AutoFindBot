using System.Net.Http.Headers;

namespace AutoFindBot.Exceptions;

public class SuccessSolutionException : Exception
{
    public HttpResponseHeaders Headers;
    
    public SuccessSolutionException(HttpResponseHeaders headers)
    {
        Headers = headers;
    }
}