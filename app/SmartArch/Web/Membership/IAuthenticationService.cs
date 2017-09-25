namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents interface for for authentication users.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Signs the user in. Sets the authentication cookie.
        /// </summary>
        /// <param name="user">The membership user.</param>
        void SignIn(IMembershipUser user);

        /// <summary>
        /// Signs the user out.
        /// </summary>
        void SignOut();
    }
}