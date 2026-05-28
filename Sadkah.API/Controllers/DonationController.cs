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
    public class DonationsController : ControllerBase
    {
        private readonly IDonationRepository _donationRepository;
        private readonly ICampaignRepository _campaignRepository;
        public DonationsController(IDonationRepository donationRepository, ICampaignRepository campaignRepository)
        {
            _donationRepository = donationRepository;
            _campaignRepository = campaignRepository;
        }

        [HttpGet]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> GetAllDonations([FromQuery] QueryObject query)
        {
            try
            {
                var donations = await _donationRepository.GetAllDonationsAsync(query);
                var donationDtos = donations.Items.Select(d => d.ToDonationDto());

                if (!donationDtos.Any()) return NotFound(ApiResponse<object>.FailResponse("Donations not found."));

                var metadata = new
                {
                    totalCount = donations.TotalCount,
                    pageSize = donations.PageSize,
                    currentPage = donations.CurrentPage,
                    totalPages = donations.TotalPages
                }; 

                return Ok(ApiResponse<IEnumerable<DonationDto>>.SuccessResponse("Donations retrieved successfully.", donationDtos, metadata));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving donations: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while retrieving donations."));
            }
        }

        [HttpGet("{id:guid}")]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> GetDonationById([FromRoute] Guid id)
        {
            try
            {
                var donation = await _donationRepository.GetDonationByIdAsync(id);
                if (donation == null) return NotFound(ApiResponse<object>.FailResponse("Donation not found."));
                return Ok(ApiResponse<DonationDto>.SuccessResponse("Donation retrieved successfully.", donation.ToDonationDto()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving donation: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while retrieving donation."));
            }
            
        }

        [HttpPost]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> CreateDonation([FromBody] CreateDonationRequestDto createDto)
        {
            try
            {
                var isCampaignExisting = await _campaignRepository.IsCampaignExistingAsync(createDto.CampaignId);

                if (!isCampaignExisting) return BadRequest(ApiResponse<object>.FailResponse("Campaign does not exist."));

                var donation = createDto.ToDonationFromCreateDto();
                var createdDonation = await _donationRepository.CreateDonationAsync(donation);

                if (createdDonation == null) return NotFound(ApiResponse<object>.FailResponse("Failed to create donation."));

                return CreatedAtAction(
                    nameof(GetDonationById),
                    new { id = createdDonation.Id },
                    ApiResponse<DonationDto>.SuccessResponse("Donation created successfully.", createdDonation.ToDonationDto())
                ); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating donation: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while creating donation."));
            }
            
        }

        [HttpPatch("{id:guid}/anonymous")]
        [EnableRateLimiting("api")]        
        [Authorize]
        public async Task<IActionResult> UpdateAnonymousDonation([FromRoute] Guid id, [FromBody] UpdateAnonymousDonationRequestDto updateDto)
        {
            try
            {
                var updatedDonation = await _donationRepository.UpdateAnonymousDonationAsync(id, updateDto.IsAnonymous!.Value);

                if (updatedDonation == null) return NotFound(ApiResponse<object>.FailResponse("Donation not found or failed to update."));

                return Ok(ApiResponse<DonationDto>.SuccessResponse("Donation anonymous status updated successfully.", updatedDonation.ToDonationDto()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating donation: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while updating the donation."));
            }
        }   
    }
}