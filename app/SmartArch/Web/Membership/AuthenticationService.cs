using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

using SmartArch.Core.Helpers;

namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents service for authentication users.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// The cookie user data service.
        /// </summary>
        private readonly ICookieUserDataService cookieUserDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="cookieUserDataService">The cookie user data service.</param>
        public AuthenticationService(ICookieUserDataService cookieUserDataService)
        {
            // Check.Require<ArgumentNullException>(cookieUserDataService != null, "Can't create AuthenticationService instance because cookie user's data service is null");
            
            this.cookieUserDataService = cookieUserDataService;
        }

        /// <summary>
        /// Signs the user in. Sets the authentication cookie.
        /// </summary>
        /// <param name="user">The membership user.</param>
        public void SignIn(IMembershipUser user)
        {
            // Check.Require<ArgumentNullException>(user != null, "Unable to sign in user because user is null");

            var cookieSection = (HttpCookiesSection)ConfigurationManager.GetSection("system.web/httpCookies");
            var authenticationSection = (AuthenticationSection)ConfigurationManager.GetSection("system.web/authentication");

            DateTime issueDate = SystemTime.Now();
            DateTime expirationDate = issueDate.AddMinutes(authenticationSection.Forms.Timeout.TotalMinutes);
            string userData = this.cookieUserDataService.CreateUserData(user);

            var authTicket = new FormsAuthenticationTicket(1, user.Login, issueDate, expirationDate, false, userData);
            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authenticateCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (cookieSection.RequireSSL || authenticationSection.Forms.RequireSSL)
            {
                authenticateCookie.Secure = true;
            }

            HttpContext.Current.Response.Cookies.Add(authenticateCookie);
        }

        /// <summary>
        /// Signs the user out.
        /// </summary>
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}