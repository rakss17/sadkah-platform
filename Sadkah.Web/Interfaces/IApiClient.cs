namespace Sadkah.Web.Interfaces
{
    public interface IApiClient
    {
        Task<ServiceResult<T>> UnauthenticatedGetAsync<T>(string requestUri);
        Task<ServiceResult<T>> UnauthenticatedPostAsync<TRequest, T>(string requestUri, TRequest request);
        Task<ServiceResult<T>> AuthenticatedGetAsync<T>(string requestUri);
        Task<ServiceResult<T>> AuthenticatedPostAsync<TRequest, T>(string requestUri, TRequest request);
    }
}
