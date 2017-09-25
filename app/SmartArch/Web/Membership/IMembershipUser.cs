namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents interface of membership user.
    /// </summary>
    public interface IMembershipUser
    {
        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        /// <value>The password as string.</value>
        string Password { get; set; }
        
        /// <summary>
        /// Gets the user's name.
        /// </summary>
        /// <value>The name of the user.</value>
        string Login { get; }

        long Id { get; }

        ///// <summary>
        ///// Gets the user's type.
        ///// </summary>
        ///// <value>The type of user.</value>
        //RoleTypes Type { get; }

        ///// <summary>
        ///// Gets the user's permissions.
        ///// </summary>
        ///// <value>The user's permissions.</value>
        //IEnumerable<Permission> Permissions { get; }
    }
}