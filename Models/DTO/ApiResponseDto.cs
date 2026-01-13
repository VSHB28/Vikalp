namespace Vikalp.Models.DTO;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public List<string> Errors { get; set; } = new List<string>();

    public static ApiResponseDto<T> SuccessResponse(T data, string message = "Success", int statusCode = 200)
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponseDto<T> ErrorResponse(string message, int statusCode = 500, List<string>? errors = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors ?? new List<string>()
        };
    }

    public static ApiResponseDto<T> ErrorResponse(List<string> errors, int statusCode = 400)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = "Validation failed",
            StatusCode = statusCode,
            Errors = errors
        };
    }
}
