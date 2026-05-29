namespace Sadkah.API.Helpers
{
    public class ServiceResult<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public object? Errors { get; set; }

        public static ServiceResult<T> Success(T data)
            => new() { Succeeded = true, Data = data };

        public static ServiceResult<T> Failure(string errorMessage, object? errors = null)
            => new() { Succeeded = false, ErrorMessage = errorMessage, Errors = errors };
    }
}
