using BlogVilla.Models;

namespace BlogVilla.Util
{
    public class Message
    {
        public static void SetMessage(HttpContext context, string message, string type)
        {
            context.Session.SetString("pageMessage", message);
            context.Session.SetString("pageMessageType", type);
        }

        public static void ResetMessage(HttpContext context)
        {
            context.Session.Remove("pageMessage");
            context.Session.Remove("pageMessageType");
        }
    }
}
