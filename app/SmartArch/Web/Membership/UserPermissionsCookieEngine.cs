using System.Security.Principal;

namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents engine for gets permissions info of user's cookie.
    /// </summary>
    public class UserPermissionsCookieEngine
    {
        /// <summary>
        /// The user data manage engine.
        /// </summary>
        private readonly ICookieUserDataService engine;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UserPermissionsCookieEngine"/> class.
        /// </summary>
        /// <param name="cookieUserDataService">The cookie user data service.</param>
        public UserPermissionsCookieEngine(ICookieUserDataService cookieUserDataService)
        {
            // Check.Require<ArgumentNullException>(cookieUserDataService != null, "Can't create instance of UserPermissionsCookieEngine because cookieUserDataService is null");
            
            this.engine = cookieUserDataService;
        }

        /// <summary>
        /// Gets the id of user.
        /// </summary>
        /// <param name="user">The user info.</param>
        /// <returns>The id of user.</returns>
        public long GetId(IPrincipal user)
        {
            // Check.Require<ArgumentNullException>(user != null, "Unable check permission in user because user's principal is null");
            
            string userData = CookieParser.GetUserData(user);
            string userIdString = this.engine.GetId(userData);
            long id = long.Parse(userIdString);

            return id;
        }        
    }
}