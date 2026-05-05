using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sadkah.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly ICampaignRepository _campaignRepository;
        public CampaignsController(ICampaignRepository campaignRepository)
        {
            _campaignRepository = campaignRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCampaigns([FromQuery] QueryObject query)
        {
            try
            {
                var campaigns = await _campaignRepository.GetAllCampaignsAsync(query);
                var campaignDtos = campaigns.Items.Select(c => c.ToCampaignDto());
                
                if (!campaignDtos.Any()) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));

                var metadata = new
                {
                    totalCount = campaigns.TotalCount,
                    pageSize = campaigns.PageSize,
                    currentPage = campaigns.CurrentPage,
                    totalPages = campaigns.TotalPages
                }; 

                return Ok(ApiResponse<IEnumerable<CampaignDto>>.SuccessResponse("Campaigns retrieved successfully.",campaignDtos, metadata));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving campaigns: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while retrieving campaigns."));
            }
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetCampaignById([FromRoute] Guid id)
        {
            try
            {
                var campaign = await _campaignRepository.GetCampaignByIdAsync(id);
                if (campaign == null) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));
                return Ok(ApiResponse<CampaignDto>.SuccessResponse("Campaign retrieved successfully.", campaign.ToCampaignDto()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while retrieving the campaign."));
            }

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCampaign([FromBody] CreateCampaignRequestDto createDto)
        {
            try
            {
                var campaign = createDto.ToCampaignFromCreateDto();
            
                var createdCampaign = await _campaignRepository.CreateCampaignAsync(campaign);

                if (createdCampaign == null) return NotFound(ApiResponse<object>.FailResponse("Failed to create campaign."));

                return CreatedAtAction(
                    nameof(GetCampaignById),
                    new { id = createdCampaign.Id },
                    ApiResponse<CampaignDto>.SuccessResponse("Campaign created successfully.", createdCampaign.ToCampaignDto())
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while creating the campaign."));
            }
           
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateCampaign([FromRoute] Guid id, [FromBody] UpdateCampaignRequestDto updateDto)
        {
            try
            {
                var updatedCampaign = await _campaignRepository.UpdateCampaignAsync(id, updateDto);

                if (updatedCampaign == null) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));

                return Ok(ApiResponse<CampaignDto>.SuccessResponse("Campaign updated successfully.", updatedCampaign.ToCampaignDto()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while updating the campaign."));
            }
            
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteCampaign([FromRoute] Guid id)
        {
            try
            {
                var deletedCampaign = await _campaignRepository.DeleteCampaignAsync(id);
            
                if (deletedCampaign == null) return NotFound(ApiResponse<object>.FailResponse("Campaign not found."));

                return Ok(ApiResponse<CampaignDto>.SuccessResponse("Campaign deleted successfully.", deletedCampaign.ToCampaignDto()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting campaign: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while deleting the campaign."));
            }

        }
    }
}