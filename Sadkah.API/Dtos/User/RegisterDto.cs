using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sadkah.API.Dtos.User
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Passwords must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Passwords must have at least one digit ('0'-'9') and one uppercase ('A'-'Z').")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public string? FirstName { get; set; } = string.Empty;
        [Required]
        public string? LastName { get; set; } = string.Empty;
        [Required]
        public UserRole? Role { get; set; }

    }
}