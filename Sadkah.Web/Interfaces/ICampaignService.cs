namespace Sadkah.Web.Interfaces
{
    public interface ICampaignService
    {
        Task<ServiceResult<PagedResult<CampaignSummary>>> GetCampaignsAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            string? category = null,
            string? location = null,
            bool? isVerified = null,
            bool mineOnly = false);
        Task<ServiceResult<CampaignModel>> CreateCampaignAsync(CampaignModel campaign);
        Task<ServiceResult<IEnumerable<CampaignCategoryModel>>> GetCampaignCategoriesAsync();
    }
}
