using System.Net.Http.Headers;

namespace AutoFindBot.Exceptions;

public class SuccessSolutionAutoRuException : Exception
{
    public HttpResponseHeaders Headers;
    public SuccessSolutionAutoRuException(HttpResponseHeaders headers)
    {
        Headers = headers;
    }
}