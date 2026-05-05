namespace Backend.PriceComparison.Api.Common.Wrappers;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public ApiResponse()
    {
    }

    public ApiResponse(T data, string? message = null)
    {
        Success = true;
        Message = message;
        Data = data;
    }

    public ApiResponse(string message)
    {
        Success = false;
        Message = message;
    }

    public ApiResponse(List<string> errors, string? message = null)
    {
        Success = false;
        Message = message;
        Errors = errors;
    }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>(data, message);
    }

    public static ApiResponse<T> ErrorResponse(string message)
    {
        return new ApiResponse<T>(message);
    }

    public static ApiResponse<T> ErrorResponse(List<string> errors, string? message = null)
    {
        return new ApiResponse<T>(errors, message);
    }
}
