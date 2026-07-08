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
        private readonly IDonationService _donationService;
        public DonationsController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        [HttpGet]
        [EnableRateLimiting("api")]
        [Authorize]
        public async Task<IActionResult> GetAllDonations([FromQuery] QueryObject query)
        {
            try
            {
                var donations = await _donationService.GetAllDonationsAsync(query);

                if (!donations.Items.Any()) return NotFound(ApiResponse<object>.FailResponse("Donations not found."));

                var metadata = new
                {
                    totalCount = donations.TotalCount,
                    pageSize = donations.PageSize,
                    currentPage = donations.CurrentPage,
                    totalPages = donations.TotalPages
                }; 

                return Ok(ApiResponse<IEnumerable<DonationDto>>.SuccessResponse("Donations retrieved successfully.", donations.Items, metadata));
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
                var donation = await _donationService.GetDonationByIdAsync(id);
                if (donation == null) return NotFound(ApiResponse<object>.FailResponse("Donation not found."));
                return Ok(ApiResponse<DonationDto>.SuccessResponse("Donation retrieved successfully.", donation));
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDonation([FromForm] CreateDonationRequestDto createDto)
        {
            try
            {
                var result = await _donationService.CreateDonationAsync(createDto);

                if (!result.Succeeded && result.ErrorMessage == "Campaign does not exist.")
                    return BadRequest(ApiResponse<object>.FailResponse(result.ErrorMessage));
                
                if (!result.Succeeded || result.Data == null)
                    return NotFound(ApiResponse<object>.FailResponse(result.ErrorMessage));

                return CreatedAtAction(
                    nameof(GetDonationById),
                    new { id = result.Data.Id },
                    ApiResponse<DonationDto>.SuccessResponse("Donation created successfully.", result.Data)
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
                var updatedDonation = await _donationService.UpdateAnonymousDonationAsync(id, updateDto);

                if (updatedDonation == null) return NotFound(ApiResponse<object>.FailResponse("Donation not found or failed to update."));

                return Ok(ApiResponse<DonationDto>.SuccessResponse("Donation anonymous status updated successfully.", updatedDonation));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating donation: {ex.Message}");
                return StatusCode(500, ApiResponse<object>.FailResponse("Internal server error while updating the donation."));
            }
        }   
    }
}
