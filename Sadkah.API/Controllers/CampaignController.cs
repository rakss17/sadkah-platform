using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Sadkah.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        public CampaignsController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> GetAllCampaigns([FromQuery] QueryObject query)
        {
            try
            {
                var campaigns = await _campaignService.GetAllCampaignsAsync(query);
                
                if (!campaigns.Items.Any()) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));

                var metadata = new
                {
                    totalCount = campaigns.TotalCount,
                    pageSize = campaigns.PageSize,
                    currentPage = campaigns.CurrentPage,
                    totalPages = campaigns.TotalPages
                }; 

                return Ok(ApiResponse<IEnumerable<CampaignDto>>.SuccessResponse("Campaigns retrieved successfully.",campaigns.Items, metadata));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving campaigns: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while retrieving campaigns."));
            }
        }

        [HttpGet("{id:guid}")]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> GetCampaignById([FromRoute] Guid id)
        {
            try
            {
                var campaign = await _campaignService.GetCampaignByIdAsync(id);
                if (campaign == null) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));
                return Ok(ApiResponse<CampaignDto>.SuccessResponse("Campaign retrieved successfully.", campaign));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while retrieving the campaign."));
            }

        }

        [HttpPost]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequestDto createDto)
        {
            try
            {
                var createdCampaign = await _campaignService.CreateCampaignAsync(createDto);

                if (createdCampaign == null) return NotFound(ApiResponse<object>.FailResponse("Failed to create campaign."));

                return CreatedAtAction(
                    nameof(GetCampaignById),
                    new { id = createdCampaign.Id },
                    ApiResponse<CampaignDto>.SuccessResponse("Campaign created successfully.", createdCampaign)
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while creating the campaign."));
            }
           
        }

        [HttpPatch("{id:guid}")]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> UpdateCampaign([FromRoute] Guid id, [FromBody] UpdateCampaignRequestDto updateDto)
        {
            try
            {
                var updatedCampaign = await _campaignService.UpdateCampaignAsync(id, updateDto);

                if (updatedCampaign == null) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));

                return Ok(ApiResponse<CampaignDto>.SuccessResponse("Campaign updated successfully.", updatedCampaign));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while updating the campaign."));
            }
            
        }

        [HttpPatch("{id:guid}/archive")]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> ArchiveCampaign([FromRoute] Guid id)
        {
            try
            {
                var archivedCampaign = await _campaignService.ArchiveCampaignAsync(id);
            
                if (archivedCampaign == null) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));

                return Ok(ApiResponse<CampaignDto>.SuccessResponse("Campaign archived successfully.", archivedCampaign));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error archiving campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while archiving the campaign."));
            }

        }

        [HttpGet("categories")]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _campaignService.GetCampaignCategoriesAsync();
                
                return Ok(ApiResponse<IEnumerable<CampaignCategoryDto>>.SuccessResponse("Campaign categories retrieved successfully.", categories));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving campaign categories: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while retrieving campaign categories."));
            }
        }
    }
}

