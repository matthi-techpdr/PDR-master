using System.Security.Principal;

namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents interface for getting user info.
    /// </summary>
    public interface IUserInfoGetter
    {
        /// <summary>
        /// Gets the info of user.
        /// </summary>
        /// <param name="user">The principal of user.</param>
        /// <returns>The info of user.</returns>
        User GetUserInfo(IPrincipal user);

        /// <summary>
        /// Gets the info of licensee user.
        /// </summary>
        /// <param name="user">The principal of user.</param>
        /// <returns>The info of user.</returns>
        LicenseeUser GetLicenseeUserInfo(IPrincipal user);
    }
}