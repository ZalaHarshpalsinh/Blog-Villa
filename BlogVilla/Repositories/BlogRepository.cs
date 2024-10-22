using BlogVilla.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace BlogVilla.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly ApplicationDbContext _context;

        public BlogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void AddBlog(Blog blog)
        {
            _context.Blogs.Add(blog);
            _context.SaveChanges();
        }

        public List<Blog> GetBlogsByUser(int userId, string filter)
        {
            var query = _context.Blogs.Include(b => b.Author).Where(b => b.AuthorId == userId);

            switch (filter.ToLower())
            {
                case "drafts":
                    query = query.Where(b => b.IsDraft && !b.IsCanceled);
                    break;
                case "canceled":
                    query = query.Where(b => b.IsCanceled);
                    break;
                default:
                    query = query.Where(b => !b.IsDraft && !b.IsCanceled);
                    break;
            }

            return query.OrderByDescending(b => b.UpdatedAt ?? b.CreatedAt).ToList();
        }

        public Blog GetBlogById(int id)
        {
            return _context.Blogs
                .Include(b => b.Likes)
                .ThenInclude(l => l.User)
                .Include(b => b.Comments)
                .ThenInclude(c => c.User)
                .Include(b => b.Author)
                .FirstOrDefault(b => b.Id == id);
        }

        public Blog UpdateBlog(Blog blog)
        {
            _context.Blogs.Update(blog);
            _context.SaveChanges();
            return blog;
        }

        public void DeleteBlog(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            _context.Blogs.Remove(blog);
            _context.SaveChanges();
        }

        public void RemoveBlog(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(b => b.Id == id);
            blog.IsCanceled = true;
            _context.Blogs.Update(blog);
            _context.SaveChanges();
        }

        public List<Blog> GetBlogs(string searchQuery, int pageNumber, int pageSize, out int totalBlogs)
        {
            var query = _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Likes)
                .Include(b => b.Comments)
                .Where(b => !b.IsDraft && !b.IsCanceled) // Exclude drafts and canceled blogs
                .AsQueryable();

            // Apply search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Content.Contains(searchQuery) ||
                    b.Author.Username.Contains(searchQuery) ||
                    b.Author.Email.Contains(searchQuery));
            }

            // Pagination
            totalBlogs = query.Count();
            return query
                .OrderByDescending(b => b.UpdatedAt ?? b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public void ToggleLike(int blogId, int userId)
        {
            // Check if the like already exists for the given blogId and userId
            var existingLike = _context.Likes
                .FirstOrDefault(l => l.BlogId == blogId && l.UserId == userId);

            if (existingLike != null)
            {
                // If a like exists, remove it
                _context.Likes.Remove(existingLike);
            }
            else
            {
                // If no like exists, add a new like
                var newLike = new Like
                {
                    BlogId = blogId,
                    UserId = userId,
                };

                _context.Likes.Add(newLike);
            }

            // Save changes to the database
            _context.SaveChanges();
        }

        public void AddComment(int blogId, int userId, string comment)
        {
            // Check if the blog exists
            var blogExists = _context.Blogs.Any(b => b.Id == blogId);
            if (!blogExists)
            {
                throw new ArgumentException("The specified blog does not exist.");
            }

            // Create a new Comment object
            var newComment = new Comment
            {
                BlogId = blogId,
                UserId = userId,
                Content = comment,
                CreatedAt = DateTime.Now // Set the creation time
            };

            // Add the new comment to the Comments DbSet
            _context.Comments.Add(newComment);

            // Save changes to the database
            _context.SaveChanges();
        }

        public Comment GetCommentById(int id)
        {
            return _context.Comments
                .Include(c => c.User)
                .Include(c => c.Blog)
                .FirstOrDefault(c => c.Id == id);
        }

        public void DeleteComment(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            _context.Comments.Remove(comment);
            _context.SaveChanges();
        }
    }
}
