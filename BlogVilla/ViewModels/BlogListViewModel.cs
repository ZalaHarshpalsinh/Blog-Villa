using BlogVilla.Models;

namespace BlogVilla.ViewModels
{
    public class BlogListViewModel
    {
        public string SearchQuery { get; set; } // The search term entered by the user
        public List<Blog> Blogs { get; set; } // List of blog previews for display

        public int CurrentPage { get; set; } // Current page number
        public int TotalPages { get; set; } // Total number of pages

        public bool HasPreviousPage => CurrentPage > 1; // Check if a previous page exists
        public bool HasNextPage => CurrentPage < TotalPages; // Check if a next page exists
    }
}
