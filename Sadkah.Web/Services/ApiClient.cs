using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Sadkah.Web.Services
{
    public sealed class ApiClient : IApiClient
    {
        private const string ClientName = "SadkahApi";
        private readonly IHttpClientFactory httpClientFactory;

        public ApiClient(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<ServiceResult<T>> GetAsync<T>(string requestUri, string? bearerToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            AddAuthorizationHeader(request, bearerToken);

            return await SendAsync<T>(request);
        }

        public async Task<ServiceResult<T>> PostAsync<TRequest, T>(string requestUri, TRequest requestBody, string? bearerToken = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = JsonContent.Create(requestBody)
            };
            AddAuthorizationHeader(request, bearerToken);

            return await SendAsync<T>(request);
        }

        private async Task<ServiceResult<T>> SendAsync<T>(HttpRequestMessage request)
        {
            try
            {
                var client = httpClientFactory.CreateClient(ClientName);
                var response = await client.SendAsync(request);
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

                if (!response.IsSuccessStatusCode || apiResponse is not { Success: true, Data: not null })
                {
                    return ServiceResult<T>.Fail(apiResponse?.Message ?? "The Sadkah API could not complete the request.");
                }

                return ServiceResult<T>.Ok(apiResponse.Data, apiResponse.Message);
            }
            catch (HttpRequestException)
            {
                return ServiceResult<T>.Fail("Could not reach the Sadkah API. Make sure Sadkah.API is running.");
            }
        }

        private static void AddAuthorizationHeader(HttpRequestMessage request, string? bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }
    }
}
