namespace Sadkah.Web.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IApiClient apiClient;
        private readonly IAuthSessionService authSession;

        public AuthService(IApiClient apiClient, IAuthSessionService authSession)
        {
            this.apiClient = apiClient;
            this.authSession = authSession;
        }

        public async Task<ServiceResult<AuthResult>> LoginAsync(LoginRequest request)
        {
            var result = await apiClient.PostAsync<LoginRequest, AuthResult>("api/user/login", request, requiresAuthentication: false);
            await SaveSessionOnSuccessAsync(result);

            return result;
        }

        public async Task<ServiceResult<AuthResult>> SignupAsync(SignupRequest request)
        {
            var result = await apiClient.PostAsync<SignupRequest, AuthResult>("api/user/register", request, requiresAuthentication: false);
            await SaveSessionOnSuccessAsync(result);

            return result;
        }

        private async Task SaveSessionOnSuccessAsync(ServiceResult<AuthResult> result)
        {
            if (result is { Success: true, Data: not null })
            {
                await authSession.SaveAsync(result.Data);
            }
        }
    }
}
