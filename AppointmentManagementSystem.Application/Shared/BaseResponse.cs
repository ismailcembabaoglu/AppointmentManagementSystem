namespace AppointmentManagementSystem.Application.Shared
{
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        // Başarılı response
        public static BaseResponse<T> SuccessResponse(T data, string message = "")
        {
            return new BaseResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        // Hatalı response
        public static BaseResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }

        // Validation hataları için
        public static BaseResponse<T> ValidationErrorResponse(List<string> errors)
        {
            return new BaseResponse<T>
            {
                Success = false,
                Message = "Doğrulama hatası oluştu.",
                Errors = errors
            };
        }
    }
}
