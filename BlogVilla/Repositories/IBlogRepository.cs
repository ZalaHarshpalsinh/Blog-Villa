using BlogVilla.Models;

namespace BlogVilla.Repositories
{
    public interface IBlogRepository
    {
        public void AddBlog(Blog blog);

        public List<Blog> GetBlogsByUser(int userId, string filter);

        public Blog GetBlogById(int id);

        public Blog UpdateBlog(Blog blog);

        public void DeleteBlog(int id);

        public void RemoveBlog(int id);

        public List<Blog> GetBlogs(string searchQuery, int pageNumber, int pageSize, out int totalBlogs);

        public void ToggleLike(int blogId, int userId);

        public void AddComment(int blogId, int userId, string comment);

        public Comment GetCommentById(int id);

        public void DeleteComment(int id);
    }
}
