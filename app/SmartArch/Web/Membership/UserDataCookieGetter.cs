using System.Security.Principal;
using System.Web.Security;

namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents class for getting user data from cookie.
    /// </summary>
    public class CookieParser
    {
        /// <summary>
        /// Gets the user data.
        /// </summary>
        /// <param name="user">The principal of user.</param>
        /// <returns>The user data from cookie.</returns>
        public static string GetUserData(IPrincipal user)
        {
            var formsIdentity = user.Identity as FormsIdentity;
            var userData = string.Empty;
            if (formsIdentity != null && !string.IsNullOrEmpty(formsIdentity.Ticket.UserData))
            {
                userData = formsIdentity.Ticket.UserData;
            }

            return userData;
        }
    }
}