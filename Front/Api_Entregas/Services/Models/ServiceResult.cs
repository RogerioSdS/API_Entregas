namespace Api_Entregas.Services.Models
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public int StatusCode { get; set; }

        public static ServiceResult<T> SuccessResult(T data, string? message = null)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200
            };
        }

        public static ServiceResult<T> SuccessMessage(string message)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Data = default,
                Message = message,
                StatusCode = 200
            };
        }

        public static ServiceResult<T> ErrorResult(string errorMessage, int statusCode = 500)
        {
            return new ServiceResult<T>
            {
                Success = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }
}
