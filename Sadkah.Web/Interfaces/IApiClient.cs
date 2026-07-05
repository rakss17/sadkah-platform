namespace Sadkah.Web.Interfaces
{
    public interface IApiClient
    {
        Task<ServiceResult<T>> GetAsync<T>(string requestUri, bool requiresAuthentication = true);
        Task<ServiceResult<T>> PostAsync<TRequest, T>(string requestUri, TRequest requestBody, bool requiresAuthentication = true);
        Task<ServiceResult<T>> PostMultipartAsync<T>(string requestUri, Func<MultipartFormDataContent> createContent, bool requiresAuthentication = true);
    }
}
