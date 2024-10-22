using BlogVilla.Models;

namespace BlogVilla.Repositories
{
    public interface IUserRepository
    {
        bool IsUsernameTaken(string username);
        bool IsEmailTaken(string email);
        User FindByUsernameAndPassword(string username, string password);
        User GetUserById(int id);
        void AddUser(User user);
        User UpdateUser(User user);
        void DeleteUser(int id);
        AdminAction AddBlogCancelLog(int blogId, int adminId);
        AdminAction AddCommentDeleteLog(Comment comment, int adminId);
        IEnumerable<AdminAction> GetAdminActionsByUserId(int userId);
    }

}
