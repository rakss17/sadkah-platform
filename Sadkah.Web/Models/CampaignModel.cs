using System.ComponentModel.DataAnnotations;

namespace Sadkah.Web.Models
{
    public class CampaignModel : IValidatableObject
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Campaign title is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Campaign title must be between 5 and 100 characters.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campaign description is required.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Campaign description must be between 10 and 200 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335", ErrorMessage = "Fundraising goal must be greater than 0.")]
        public decimal TargetAmount { get; set; }

        public decimal CurrentAmount { get; set; }
        public DateTime Deadline { get; set; }
        public CampaignStatus Status { get; set; }
        public bool IsVerified { get; set; }
        public string OwnerName { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Deadline == default)
            {
                yield return new ValidationResult(
                    "Campaign end date is required.",
                    [nameof(Deadline)]);
            }
            else if (Deadline.Date <= DateTime.Today)
            {
                yield return new ValidationResult(
                    "Campaign end date must be in the future.",
                    [nameof(Deadline)]);
            }
        }
    }
}
