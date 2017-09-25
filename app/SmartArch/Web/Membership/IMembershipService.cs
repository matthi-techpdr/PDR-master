namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents interface for working with membership.
    /// </summary>
    /// <typeparam name="T">The membership user type.</typeparam>
    public interface IMembershipService<out T> where T : IMembershipUser
    {
        /// <summary>
        /// Validate login and password.
        /// </summary>
        /// <param name="login">Login of the user.</param>
        /// <param name="password">Password of the user.</param>
        /// <returns>
        ///     <c>true</c> if login and password is valid; otherwise, <c>false</c>.
        /// </returns>
        T GetUser(string login, string password);

        /// <summary>
        /// Change password.
        /// </summary>
        /// <param name="login">Login of the user.</param>
        /// <param name="oldPassword">Old password of the user.</param>
        /// <param name="newPassword">New password of the user.</param>
        void ChangePassword(string login, string oldPassword, string newPassword);
    }
}