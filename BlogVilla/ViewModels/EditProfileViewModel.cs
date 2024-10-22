using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BlogVilla.ViewModels
{
    public class EditProfileViewModel
    {
        public int Id { get; set; } // User's ID, hidden in the form for identification

        [Required]
        public string Username { get; set; } // Editable Username

        [Required, EmailAddress]
        public string Email { get; set; } // Editable Email

        [Display(Name = "Profile Photo")]
        public IFormFile? ProfilePhoto { get; set; } // New Profile Photo upload option (optional)

        public string? ExistingProfilePhoto { get; set; } // To retain current profile photo if not 
    }

}
