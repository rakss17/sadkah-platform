namespace Sadkah.API.Interfaces
{
    public interface ICampaignService
    {
        Task<PagedResult<CampaignDto>> GetAllCampaignsAsync(QueryObject query);
        Task<CampaignDto?> GetCampaignByIdAsync(Guid id);
        Task<CampaignDto?> CreateCampaignAsync(CreateCampaignRequestDto createDto);
        Task<CampaignDto?> UpdateCampaignAsync(Guid id, UpdateCampaignRequestDto updateDto);
        Task<CampaignDto?> ArchiveCampaignAsync(Guid id);
        Task<IEnumerable<CampaignCategoryDto>> GetCampaignCategoriesAsync();
    }
}
