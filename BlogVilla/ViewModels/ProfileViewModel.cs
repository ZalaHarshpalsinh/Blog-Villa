using BlogVilla.Models;

namespace BlogVilla.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool IsAdmin { get; set; }
    }

}
