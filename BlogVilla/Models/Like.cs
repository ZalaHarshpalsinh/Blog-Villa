namespace BlogVilla.Models
{
    public class Like
    {
        public int Id { get; set; } // Primary Key

        public int BlogId { get; set; } // Foreign key to the blog
        public Blog Blog { get; set; } // Navigation property for the Blog

        public int UserId { get; set; } // Foreign key to the user who liked the blog
        public User User { get; set; } // Navigation property for the User
    }
}
