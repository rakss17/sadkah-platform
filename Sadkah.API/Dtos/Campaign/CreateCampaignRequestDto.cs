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
    }
}