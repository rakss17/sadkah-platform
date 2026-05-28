using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Sadkah.Web.Services
{
    public sealed class ApiClient : IApiClient
    {
        private const string ClientName = "SadkahApi";
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IAuthSessionService authSession;

        public ApiClient(IHttpClientFactory httpClientFactory, IAuthSessionService authSession)
        {
            this.httpClientFactory = httpClientFactory;
            this.authSession = authSession;
        }

        public async Task<ServiceResult<T>> AuthenticatedGetAsync<T>(string requestUri)
        {
            var token = await authSession.GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("An access token is required to make API requests. Ensure the user is authenticated.");
            }

            return await SendAsync<T>(() =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                AddAuthorizationHeader(request, token);
                return request;
            }, token);
        }

        public async Task<ServiceResult<T>> AuthenticatedPostAsync<TRequest, T>(string requestUri, TRequest requestBody)
        {
            var token = await authSession.GetAccessTokenAsync();

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("An access token is required to make API requests. Ensure the user is authenticated.");
            }

            return await SendAsync<T>(() =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = JsonContent.Create(requestBody)
                };
                AddAuthorizationHeader(request, token);
                return request;
            }, token);
        }

        public async Task<ServiceResult<T>> UnauthenticatedGetAsync<T>(string requestUri)
        {

            return await SendAsync<T>(() =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                return request;
            }, null);
        }

        public async Task<ServiceResult<T>> UnauthenticatedPostAsync<TRequest, T>(string requestUri, TRequest requestBody)
        {
            return await SendAsync<T>(() =>
            {
                var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = JsonContent.Create(requestBody)
                };
                return request;
            }, null);
        }

        private async Task<ServiceResult<T>> SendAsync<T>(Func<HttpRequestMessage> createRequest, string? bearerToken)
        {
            try
            {
                var client = httpClientFactory.CreateClient(ClientName);
                var response = await client.SendAsync(createRequest());

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized &&
                    !string.IsNullOrWhiteSpace(bearerToken) &&
                    await TryRefreshTokenAsync())
                {
                    var retryRequest = createRequest();
                    AddAuthorizationHeader(retryRequest, await authSession.GetAccessTokenAsync());
                    response = await client.SendAsync(retryRequest);
                }

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

        private async Task<bool> TryRefreshTokenAsync()
        {
            var refreshToken = await authSession.GetRefreshTokenAsync();

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            var client = httpClientFactory.CreateClient(ClientName);
            var response = await client.PostAsJsonAsync("api/user/refresh-token", new RefreshTokenRequest(refreshToken));

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<TokenRefreshResult>>();

            if (result?.Data is null ||
                string.IsNullOrWhiteSpace(result.Data.AccessToken) ||
                string.IsNullOrWhiteSpace(result.Data.RefreshToken))
            {
                return false;
            }

            await authSession.SaveTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
            return true;
        }

        private static void AddAuthorizationHeader(HttpRequestMessage request, string? bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return;
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        private sealed record RefreshTokenRequest(string RefreshToken);

        private sealed class TokenRefreshResult
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
        }
    }
}
