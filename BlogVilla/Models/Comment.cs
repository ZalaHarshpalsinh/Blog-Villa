namespace BlogVilla.Models
{
    public class Comment
    {
        public int Id { get; set; } // Primary Key

        public string Content { get; set; } // The text of the comment
        public DateTime CreatedAt { get; set; } // When the comment was created

        public int BlogId { get; set; } // Foreign key to the blog
        public Blog Blog { get; set; } // Navigation property for the Blog

        public int UserId { get; set; } // Foreign key to the user who posted the comment
        public User User { get; set; } // Navigation property for the User
    }
}
