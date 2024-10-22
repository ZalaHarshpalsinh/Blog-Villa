using BlogVilla.Models;

namespace BlogVilla.ViewModels
{
    public class UserBlogsViewModel
    {
        public User Owner { get; set; }
        public List<Blog> Blogs { get; set; } // List of blogs to display

        public bool IsCurrentUser { get; set; } // Check if the current user is viewing their own blog list

        public string SelectedTab { get; set; } // To track which tab is selected (Published, Drafts, Canceled)
    }
}
