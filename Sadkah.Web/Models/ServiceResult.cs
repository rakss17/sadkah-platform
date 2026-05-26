namespace Sadkah.Web.Models
{
    public sealed class ServiceResult<T>
    {
        private ServiceResult(bool success, T? data, string message, bool requiresAuthentication)
        {
            Success = success;
            Data = data;
            Message = message;
            RequiresAuthentication = requiresAuthentication;
        }

        public bool Success { get; }
        public T? Data { get; }
        public string Message { get; }
        public bool RequiresAuthentication { get; }

        public static ServiceResult<T> Ok(T data, string message = "")
        {
            return new ServiceResult<T>(true, data, message, false);
        }

        public static ServiceResult<T> Fail(string message)
        {
            return new ServiceResult<T>(false, default, message, false);
        }

        public static ServiceResult<T> AuthenticationRequired()
        {
            return new ServiceResult<T>(false, default, "Please sign in to continue.", true);
        }
    }
}
