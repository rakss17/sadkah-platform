namespace Sadkah.Web.Interfaces
{
    public interface ICampaignService
    {
        Task<ServiceResult<IReadOnlyList<CampaignSummary>>> GetCampaignsAsync(int pageSize = 50, string? searchTerm = null);
        Task<ServiceResult<CampaignModel>> CreateCampaignAsync(CampaignModel campaign);
    }
}
