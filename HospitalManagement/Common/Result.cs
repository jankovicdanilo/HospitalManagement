namespace HospitalManagement.Common
{
    public enum ErrorType
    {
        NotFound,
        Validation,
        Conflict
    }

    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? ErrorCode { get; set; }
        public ErrorType? ErrorType { get; set; }
        public T? Data { get; set; }

        public static Result<T> Ok(T data, string message = "") => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

        public static Result<T> Fail(string message, string? errorCode = null, 
            ErrorType? errorType = null) => new()
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            ErrorType = errorType
        };
    }

    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorCode { get; set; }
        public ErrorType? ErrorType { get; set; }

        public static Result Ok(string message = "") => new()
        {
            Success = true,
            Message = message
        };

        public static Result Fail(string message, string? errorCode = null, 
            ErrorType? errorType = null) => new()
        {
            Success = false,
            Message = message,
            ErrorCode = errorCode,
            ErrorType = errorType
        };
    }
}
