namespace BlogVilla.Models
{
    public class AdminAction
    {
        public int Id { get; set; } // Primary Key

        public string ActionDescription { get; set; } // Description of the admin action
        public DateTime ActionDate { get; set; } // When the action was taken

        public int UserId { get; set; } // Foreign key to the affected user
        public User User { get; set; } // Navigation property for the User
        public int AdminId { get; set; } // Foreign key to the admin who took the action
        public User Admin { get; set; } // Navigation property for the Admin (User entity)
    }
}
