namespace BlogVilla.Models
{
    public class User
    {
        public int Id { get; set; } // Primary Key
        public string Username { get; set; } // User's username
        public string Email { get; set; } // User's email
        public string Password { get; set; } // User's password (not hashed)
        public bool IsAdmin { get; set; } // Indicates if the user is an admin
        public string ProfilePhoto { get; set; } // URL or path to the user's profile photo
    }

}
