namespace Sadkah.Web.Models
{
    public sealed class ServiceResult<T>
    {
        private ServiceResult(bool success, T? data, string message, bool requiresAuthentication, JsonElement? metadata = null)
        {
            Success = success;
            Data = data;
            Message = message;
            RequiresAuthentication = requiresAuthentication;
            Metadata = metadata;
        }

        public bool Success { get; }
        public T? Data { get; }
        public string Message { get; }
        public bool RequiresAuthentication { get; }
        public JsonElement? Metadata { get; }

        public static ServiceResult<T> Ok(T data, string message = "", JsonElement? metadata = null)
        {
            return new ServiceResult<T>(true, data, message, false, metadata);
        }

        public static ServiceResult<T> Fail(string message)
        {
            return new ServiceResult<T>(false, default, message, false);
        }

        public static ServiceResult<T> AuthenticationRequired(string message = "Please sign in to continue.")
        {
            return new ServiceResult<T>(false, default, message, true);
        }
    }
}
