using System;
using System.Security.Principal;

namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents user info getter.
    /// </summary>
    public class UserInfoGetter : IUserInfoGetter
    {
        /// <summary>
        /// The cookie user data service.
        /// </summary>
        private readonly ICookieUserDataService cookieUserDataService;

        /// <summary>
        /// The users repository.
        /// </summary>
        private readonly IRepository<User> usersRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfoGetter"/> class.
        /// </summary>
        /// <param name="cookieUserDataService">The cookie user data service.</param>
        /// <param name="usersRepository">The users repository.</param>
        public UserInfoGetter(ICookieUserDataService cookieUserDataService, IRepository<User> usersRepository)
        {
            Check.Require<ArgumentNullException>(cookieUserDataService != null, "Can't create instance of UserInfoGetter if cookieUserDataService is null");
            Check.Require<ArgumentNullException>(usersRepository != null, "Can't create instance of UserInfoGetter if usersRepository is null");
            
            this.cookieUserDataService = cookieUserDataService;
            this.usersRepository = usersRepository;
        }

        /// <summary>
        /// Gets the info of user.
        /// </summary>
        /// <param name="user">The principal of user.</param>
        /// <returns>The info of user.</returns>
        public User GetUserInfo(IPrincipal user)
        {
            string userIdString = this.cookieUserDataService.GetId(CookieParser.GetUserData(user));
            int userId;
            User userInfo = null;
            if (int.TryParse(userIdString, out userId))
            {
                userInfo = this.usersRepository.Find.ById(userId);
            }

            return userInfo;
        }

        /// <summary>
        /// Gets the info of licensee user.
        /// </summary>
        /// <param name="user">The principal of user.</param>
        /// <returns>The info of user.</returns>
        public LicenseeUser GetLicenseeUserInfo(IPrincipal user)
        {
            var userInfo = this.GetUserInfo(user) as LicenseeUser;

            Check.Require<ArgumentNullException>(userInfo != null, "Unable get licensee user because user is not LicenseeUser");

            return userInfo;
        }
    }
}