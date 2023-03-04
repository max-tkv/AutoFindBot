namespace AutoFindBot.Utils.ApiResponse;

public class ApiResponseResult<T> : ApiResponse where T : class
{
    public ApiResponseResult()
    {
    }

    public ApiResponseResult(T result) => this.Result = result;

    public T? Result { get; set; }
}