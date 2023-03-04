
namespace AutoFindBot.Utils.ApiResponse;

public class ApiResponse
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public int ErrorCode { get; set; }

    public static ApiResponse CreateSuccess() => new ApiResponse()
    {
        Success = true
    };

    public static ApiResponseResult<TResult> CreateSuccess<TResult>(TResult result) where TResult : class
    {
        ApiResponseResult<TResult> success = new ApiResponseResult<TResult>();
        success.Result = result;
        success.Success = true;
        return success;
    }

    public static ApiResponse CreateFailure(string? error = null, int? errorCode = null) => new ApiResponse()
    {
        Success = false,
        Error = error,
        ErrorCode = errorCode ?? 0
    };

    public static ApiResponseResult<TResult> CreateFailure<TResult>(
        string? error = null,
        int? errorCode = null)
        where TResult : class
    {
        ApiResponseResult<TResult> failure = new ApiResponseResult<TResult>();
        failure.Success = false;
        failure.Error = error;
        failure.ErrorCode = errorCode ?? 0;
        return failure;
    }
}