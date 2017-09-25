namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents interface for storing and selecting user data in cookie.
    /// </summary>
    public interface ICookieUserDataService
    {
        /// <summary>
        /// Creates the user data for storing in cookie.
        /// </summary>
        /// <param name="user">The membership user.</param>
        /// <returns>The user data as string.</returns>
        string CreateUserData(IMembershipUser user);

        /// <summary>
        /// Gets the id of user.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <returns>The id of user.</returns>
        string GetId(string userData);
    }
}