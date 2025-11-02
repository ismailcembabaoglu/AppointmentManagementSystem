namespace AppointmentManagementSystem.Application.Shared
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result<T> SuccessResult(T data, string message = "İşlem başarılı")
        {
            return new Result<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static Result<T> FailureResult(string message, List<string>? errors = null)
        {
            return new Result<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
