using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Dtos.Campaign
{
    public class CreateCampaignRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "OwnerId must be between 5 and 100 characters.")]
        public string OwnerId { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters.")]
        public string? Title { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Description must be between 5 and 200 characters.")]
        public string? Description { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "TargetAmount must be a positive decimal.")]
        public decimal TargetAmount { get; set; }
        [Required]
        public DateTime? Deadline { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        [Required(ErrorMessage = "Address Line 1 is required.")]
        public string AddressLine1 { get; set; } = string.Empty;
        public string? AddressLine2 { get; set; }
        [Required(ErrorMessage = "Barangay is required.")]
        public string Barangay { get; set; } = string.Empty;
        [Required(ErrorMessage = "City/Municipality is required.")]
        public string City { get; set; } = string.Empty;
        [Required(ErrorMessage = "Province is required.")]
        public string Province { get; set; } = string.Empty;
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; } = string.Empty;
    }
}