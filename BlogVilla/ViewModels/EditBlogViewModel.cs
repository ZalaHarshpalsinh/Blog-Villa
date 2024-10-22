using System.ComponentModel.DataAnnotations;

namespace BlogVilla.ViewModels
{
    public class EditBlogViewModel
    {
        public int Id { get; set; } // User's ID, hidden in the form for identification

        [Required]
        public string Title { get; set; } // Editable Username

        [Required]
        public string Content { get; set; } // Editable Email

        [Required]
        public bool IsDraft { get; set; }

        [Display(Name = "Profile Photo")]
        public IFormFile? CoverPhoto { get; set; } // New Profile Photo upload option (optional)

        public string? ExistingCoverPhoto { get; set; } // To retain current profile photo if not 
    }
}
