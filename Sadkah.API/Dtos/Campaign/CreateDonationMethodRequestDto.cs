using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Sadkah.API.Dtos.Campaign
{
    public class CreateDonationMethodRequestDto
    {
        [Required(ErrorMessage = "Method type is required.")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Provider is required.")]
        public string Provider { get; set; } = string.Empty;

        [Required(ErrorMessage = "QR code image is required.")]
        public IFormFile QrImageFile { get; set; } = null!;
    }
}