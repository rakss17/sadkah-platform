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

        public async Task<ServiceResult<IReadOnlyList<CampaignSummary>>> GetCampaignsAsync(int pageSize = 50)
        {
            var result = await apiClient.AuthenticatedGetAsync<List<CampaignResponse>>($"api/campaigns?pageSize={pageSize}");

            if (!result.Success || result.Data is null)
            {
                return ServiceResult<IReadOnlyList<CampaignSummary>>.Fail(result.Message);
            }

            var currentUserFullName = await authSession.GetCurrentUserFullNameAsync();
            var campaigns = result.Data
                .Select(campaign => MapCampaign(campaign, currentUserFullName))
                .ToList();

            return ServiceResult<IReadOnlyList<CampaignSummary>>.Ok(campaigns, result.Message);
        }

        private static CampaignSummary MapCampaign(CampaignResponse campaign, string? currentUserFullName)
        {
            return new CampaignSummary(
                campaign.Title,
                GetStatusLabel(campaign.Status),
                campaign.OwnerName,
                GetDaysLeft(campaign.Deadline),
                campaign.CurrentAmount,
                campaign.TargetAmount,
                campaign.IsVerified,
                IsCurrentUsersCampaign(campaign.OwnerName, currentUserFullName));
        }

        private static bool IsCurrentUsersCampaign(string ownerName, string? currentUserFullName)
        {
            return !string.IsNullOrWhiteSpace(currentUserFullName)
                && string.Equals(ownerName, currentUserFullName, StringComparison.OrdinalIgnoreCase);
        }

        private static int GetDaysLeft(DateTime deadline)
        {
            return Math.Max(0, (deadline.Date - DateTime.Today).Days);
        }

        private static string GetStatusLabel(int status)
        {
            return status switch
            {
                0 => "Pending",
                1 => "Active",
                2 => "Completed",
                3 => "Rejected",
                _ => "Campaign"
            };
        }
    }

    public sealed record CampaignSummary(
        string Title,
        string Category,
        string Location,
        int DaysLeft,
        decimal RaisedAmount,
        decimal GoalAmount,
        bool IsVerified,
        bool IsMine);

    internal sealed class CampaignResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal TargetAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public DateTime Deadline { get; set; }
        public int Status { get; set; }
        public bool IsVerified { get; set; }
        public string OwnerName { get; set; } = string.Empty;
    }
}
