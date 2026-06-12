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
    }
}
