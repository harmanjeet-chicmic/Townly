namespace RealEstateInvesting.API.Contracts;

public class ApiResponse<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<ApiError>? Errors { get; set; }

    public static ApiResponse<T> Success(T data, string message = "Success", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    public static ApiResponse<T> Failure(
        string message,
        int statusCode,
        List<ApiError>? errors = null)
    {
        return new ApiResponse<T>
        {
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}
