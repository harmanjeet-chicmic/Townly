namespace RealEstateInvesting.API.Contracts;

public class AdminApiResponse<T>
{
    public int StatusCode { get; set; }

    public bool Status { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public T? Data { get; set; }

    // public List<ApiError>? Errors { get; set; }

    public static AdminApiResponse<T> Success(
        T data,
        string message = "Operation completed successfully",
        int statusCode = 200)
    {
        return new AdminApiResponse<T>
        {
            StatusCode = statusCode,
            Status = true,
            Type = "SUCCESS",
            Message = message,
            Data = data
        };
    }

    public static AdminApiResponse<T> Failure(
        string message,
        int statusCode,
        List<ApiError>? errors = null)
    {
        return new AdminApiResponse<T>
        {
            StatusCode = statusCode,
            Status = false,
            Type = "ERROR",
            Message = message,
            Data = default
        };
    }
}