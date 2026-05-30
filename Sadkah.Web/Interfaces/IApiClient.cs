namespace Sadkah.Web.Interfaces
{
    public interface IApiClient
    {
        Task<ServiceResult<T>> GetAsync<T>(string requestUri, bool requiresAuthentication = true);
        Task<ServiceResult<T>> PostAsync<TRequest, T>(string requestUri, TRequest requestBody, bool requiresAuthentication = true);
    }
}
