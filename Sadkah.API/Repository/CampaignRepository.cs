using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Repository
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly ApplicationDBContext _context;
        public CampaignRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Campaign>> GetAllCampaignsAsync(QueryObject query)
        {
            var campaignsQuery = _context.Campaigns.Where(c => !c.IsArchived).Include(c => c.Owner).Include(c => c.Donations).ThenInclude(d => d.Donor).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                campaignsQuery = campaignsQuery.Where(c => c.OwnerId == query.UserId).OrderByDescending(c => c.CreatedAt);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                campaignsQuery = campaignsQuery.Where(c => c.Title.Contains(query.SearchTerm) || c.Description.Contains(query.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    campaignsQuery = query.IsSortDescending ? campaignsQuery.OrderByDescending(c => c.Title) : campaignsQuery.OrderBy(c => c.Title);
                }
                else if (query.SortBy.Equals("deadline", StringComparison.OrdinalIgnoreCase))
                {
                    campaignsQuery = query.IsSortDescending ? campaignsQuery.OrderByDescending(c => c.Deadline) : campaignsQuery.OrderBy(c => c.Deadline);
                }
                else if (query.SortBy.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
                {
                    campaignsQuery = query.IsSortDescending ? campaignsQuery.OrderByDescending(c => c.CreatedAt) : campaignsQuery.OrderBy(c => c.CreatedAt);
                }
            }
            else
            {
                campaignsQuery = campaignsQuery.OrderByDescending(c => c.CreatedAt);
            }
            
            var totalCount = await campaignsQuery.CountAsync();

            var pageSize = query.PageSize > 0 ? query.PageSize : 10;
            var pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;

            var items = await campaignsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Campaign>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<Campaign?> GetCampaignByIdAsync(Guid id)
        {
            return await _context.Campaigns.Include(c => c.Owner).Include(c => c.Donations).ThenInclude(d => d.Donor).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Campaign> CreateCampaignAsync(Campaign campaignModel)
        {
            await _context.Campaigns.AddAsync(campaignModel);
            await _context.SaveChangesAsync();

            var createdCampaign = await _context.Campaigns.Include(c => c.Owner).FirstOrDefaultAsync(c => c.Id == campaignModel.Id);

            return createdCampaign!;

        }

        public async Task<Campaign?> UpdateCampaignAsync(Guid id, UpdateCampaignRequestDto updateCampaignDto)
        {
            var existingCampaign = await _context.Campaigns.Include(c => c.Owner).FirstOrDefaultAsync(c => c.Id == id);

            if (existingCampaign == null) return null;

            existingCampaign.Title = updateCampaignDto.Title ?? existingCampaign.Title;
            existingCampaign.OwnerId = updateCampaignDto.OwnerId ?? existingCampaign.OwnerId;
            existingCampaign.Description = updateCampaignDto.Description ?? existingCampaign.Description;
            existingCampaign.TargetAmount = updateCampaignDto.TargetAmount ?? existingCampaign.TargetAmount;
            existingCampaign.Deadline = updateCampaignDto.Deadline ?? existingCampaign.Deadline;

            await _context.SaveChangesAsync();
            
            return existingCampaign;
        }

        public async Task<Campaign?> ArchiveCampaignAsync(Guid id)
        {
            var campaign = await _context.Campaigns.FirstOrDefaultAsync(c => c.Id == id);
            if (campaign == null) return null;

            campaign.IsArchived = true;
            campaign.ArchivedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            return campaign;

        }

        public async Task<IEnumerable<CampaignCategoryDto>> GetCampaignCategoriesAsync()
        {
        return await _context.CampaignCategories
            .Select(c => new CampaignCategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
}

        public async Task<bool> IsCampaignExistingAsync(Guid id)
        {
            return await _context.Campaigns.AnyAsync(c => c.Id == id);
        }
    }
}