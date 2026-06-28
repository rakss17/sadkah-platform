namespace Sadkah.Web.Services
{
    public sealed class CampaignService : ICampaignService
    {
        private readonly IApiClient apiClient;
        private readonly IAuthSessionService authSession;

        public CampaignService(IApiClient apiClient, IAuthSessionService authSession)
        {
            this.apiClient = apiClient;
            this.authSession = authSession;
        }

        public async Task<ServiceResult<IReadOnlyList<CampaignSummary>>> GetCampaignsAsync(int pageSize = 50, string? searchTerm = null)
        {
            var url = $"api/campaigns?pageSize={pageSize}";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                url += $"&searchTerm={Uri.EscapeDataString(searchTerm)}";
            }

            var result = await apiClient.GetAsync<List<CampaignModel>>(url, requiresAuthentication: true);

            if (!result.Success || result.Data is null)
            {
                return ServiceResult<IReadOnlyList<CampaignSummary>>.Fail(result.Message);
            }

            var currentUserFullName = await authSession.GetCurrentUserFullNameAsync();
            var campaigns = result.Data
                .Select(campaign => campaign.ToSummary(currentUserFullName))
                .ToList();

            return ServiceResult<IReadOnlyList<CampaignSummary>>.Ok(campaigns, result.Message);
        }

        public async Task<ServiceResult<CampaignModel>> CreateCampaignAsync(CampaignModel campaign)
        {
            var accessToken = await authSession.GetAccessTokenAsync();
            var ownerId = GetOwnerId(accessToken);

            if (string.IsNullOrWhiteSpace(ownerId))
            {
                return ServiceResult<CampaignModel>.AuthenticationRequired();
            }

            var request = new CreateCampaignRequest(
                ownerId,
                campaign.Title,
                campaign.Description,
                campaign.TargetAmount,
                campaign.Deadline.GetValueOrDefault());

            return await apiClient.PostAsync<CreateCampaignRequest, CampaignModel>(
                "api/campaigns",
                request,
                requiresAuthentication: true);
        }

        private static string? GetOwnerId(string? accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return null;
            }

            var tokenParts = accessToken.Split('.');
            if (tokenParts.Length < 2)
            {
                return null;
            }

            try
            {
                var payload = tokenParts[1]
                    .Replace('-', '+')
                    .Replace('_', '/');
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

                using var document = System.Text.Json.JsonDocument.Parse(Convert.FromBase64String(payload));
                if (document.RootElement.TryGetProperty("sub", out var subjectClaim))
                {
                    return subjectClaim.GetString();
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private sealed record CreateCampaignRequest(
            string OwnerId,
            string Title,
            string Description,
            decimal TargetAmount,
            DateTime Deadline);
    }
}
