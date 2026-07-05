using System.ComponentModel.DataAnnotations;

namespace Sadkah.Web.Models
{
    public class CampaignModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Campaign title is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Campaign title must be between 5 and 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campaign description is required.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Campaign description must be between 10 and 200 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public Guid CategoryId { get; set; } = Guid.Empty;
        public string? CategoryName { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Fundraising goal must be greater than 0.")]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; }
        [Required(ErrorMessage = "Campaign end date is required.")]
        [FutureDate(ErrorMessage = "Campaign end date must be in the future.")]
        public DateTime? Deadline { get; set; }

        public CampaignStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public string OwnerName { get; set; } = string.Empty;

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
        public string Country { get; set; } = "Philippines";

        [MinLength(1, ErrorMessage = "At least one donation method is required.")]
        public List<DonationMethodModel> DonationMethods { get; set; } = new();
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date && date.Date <= DateTime.Today)
                return new ValidationResult(ErrorMessage, [validationContext.MemberName!]);
            return ValidationResult.Success;
        }
    }
}
