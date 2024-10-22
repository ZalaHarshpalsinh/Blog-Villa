using System.ComponentModel.DataAnnotations;

namespace BlogVilla.ViewModels
{
    public class BlogUploadViewModel
    {
        public int Id { get; set; } // Blog ID (for editing scenarios)

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        public bool IsDraft { get; set; } // Checkbox to publish or save as draft

        [Required(ErrorMessage = "Cover photo is required")]
        public IFormFile CoverPhoto { get; set; } // File upload
    }
}
