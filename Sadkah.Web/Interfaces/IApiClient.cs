namespace Sadkah.Web.Interfaces
{
    public interface IApiClient
    {
        Task<ServiceResult<T>> GetAsync<T>(string requestUri, string? bearerToken = null);
        Task<ServiceResult<T>> PostAsync<TRequest, T>(string requestUri, TRequest request, string? bearerToken = null);
    }
}
