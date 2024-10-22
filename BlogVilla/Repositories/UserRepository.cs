using BlogVilla.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogVilla.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsUsernameTaken(string username)
        {
            return _context.Users.Any(u => u.Username == username);
        }

        public bool IsEmailTaken(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }

        public User FindByUsernameAndPassword(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User UpdateUser(User newUser)
        {
            var oldUser = _context.Users.FirstOrDefault(u => u.Id == newUser.Id);
            oldUser.Username = newUser.Username;
            oldUser.Email = newUser.Email;
            oldUser.ProfilePhoto = newUser.ProfilePhoto;

            _context.SaveChanges();

            return oldUser;
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public AdminAction AddBlogCancelLog(int blogId, int adminId)
        {
            var blog = _context.Blogs
                        .Include(b => b.Author)
                        .FirstOrDefault(b => b.Id == blogId);

            string logMessage = $"Your blog with the title \"{blog.Title}\" has been canceled by the admin.";

            var adminAction = new AdminAction
                {
                    ActionDescription = logMessage,
                    ActionDate = DateTime.Now,
                    UserId = blog.AuthorId,
                    AdminId = adminId
                };

            _context.AdminActions.Add(adminAction);
            _context.SaveChanges();

            return adminAction;
        }

        public AdminAction AddCommentDeleteLog(Comment comment, int adminId)
        {
            string logMessage = $"Your comment \"{comment.Content}\", was deleted by the admin.";

            var adminAction = new AdminAction {
                ActionDescription = logMessage,
                ActionDate = DateTime.Now,
                UserId = comment.UserId,
                AdminId = adminId
            };

            _context.AdminActions.Add(adminAction);
            _context.SaveChanges();

            return adminAction;
        }

        public IEnumerable<AdminAction> GetAdminActionsByUserId(int userId)
        {
            return _context.AdminActions
            .Include(a => a.Admin)  // Load Admin details
            .Include(a => a.User)   // Load User details
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.ActionDate)
            .ToList();
        }
    }
}
