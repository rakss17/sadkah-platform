using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Interfaces
{
    public interface ICampaignRepository
    {
        Task<PagedResult<Campaign>> GetAllCampaignsAsync(QueryObject query);
        Task<Campaign?> GetCampaignByIdAsync(Guid id);
        Task<Campaign> CreateCampaignAsync(Campaign campaign);
        Task<Campaign?> UpdateCampaignAsync(Guid id, UpdateCampaignRequestDto updatedCampaign);
        Task<Campaign?> ArchiveCampaignAsync(Guid id);
        Task<IEnumerable<CampaignCategoryDto>> GetCampaignCategoriesAsync();
        Task<bool> IsCampaignExistingAsync(Guid id);
    }
}