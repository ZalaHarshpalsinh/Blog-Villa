using BlogVilla.Models;
using BlogVilla.Repositories;

namespace BlogVilla.Util
{
    public class Auth
    {
        public static bool IsLoggedIn(HttpContext context)
        {
            string userId = context.Session.GetString("userId");
            string userName = context.Session.GetString("username");

            // If both session values are not null, the user is authenticated
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userId))
            {
                return true;
            }

            // Otherwise, the user is not authenticated
            return false;
        }

        public static void Login(HttpContext context, User user)
        {
            context.Session.SetInt32("userId", user.Id);
            context.Session.SetString("username", user.Username);
            context.Session.SetString("email", user.Email);
            context.Session.SetInt32("isAdmin", user.IsAdmin ? 1 : 0 );
            context.Session.SetString("profilePhoto", user.ProfilePhoto);
        }

        public static void Logout(HttpContext context)
        {
            context.Session.Clear();
        }
    }
}
