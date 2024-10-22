using System.ComponentModel.DataAnnotations;

namespace BlogVilla.Models
{
    public class Blog
    {
        public int Id { get; set; } // Blog ID (Primary Key)

        public string Title { get; set; } // Title of the Blog

        public string Content { get; set; } // Content of the Blog

        public string CoverPhoto { get; set; } // URL or path to the user's profile photo

        public bool IsDraft { get; set; } // Indicates if the blog is saved as a draft

        public bool IsCanceled { get; set; } // Admin can cancel the blog to remove its visibility

        public DateTime CreatedAt { get; set; } // Date when the blog was created

        public DateTime? UpdatedAt { get; set; } // Optional: Date of the last update (nullable)

        public int AuthorId { get; set; } // Foreign key to the user who created the blog
        public User Author { get; set; } // Navigation property for the Author (User entity)

        public ICollection<Comment> Comments { get; set; } // Comments on this blog
        public ICollection<Like> Likes { get; set; } // Likes for this blog
    }
}