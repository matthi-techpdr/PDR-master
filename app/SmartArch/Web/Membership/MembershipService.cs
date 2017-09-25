using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartArch.Web.Membership
{
    /// <summary>
    /// Represents class for working with users
    /// </summary>
    /// <typeparam name="T">The membership user type.</typeparam>
    public class MembershipService<T> : IMembershipService<T> where T : class, IMembershipUser
    {
        /// <summary>
        /// Data access object for users.
        /// </summary>
        private readonly IDao<T> membershipDao;

        /// <summary>
        /// The encryption service.
        /// </summary>
        private readonly IPasswordEncryptionService encryptionService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MembershipService&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="membershipDao">The data access object of membership users.</param>
        /// <param name="encryptionService">The service for encrypt user password.</param>
        public MembershipService(IDao<T> membershipDao, IPasswordEncryptionService encryptionService)
        {
            Check.Require<ArgumentNullException>(membershipDao != null, "Can't create instance of MembershipService class because membershipDao is null");

            this.membershipDao = membershipDao;
            this.encryptionService = encryptionService;
        }

        /// <summary>
        /// Validate login and password
        /// </summary>
        /// <param name="login">Login of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>
        ///     <c>True</c> if login and password is valid; otherwise, <c>false</c>.
        /// </returns>
        public T GetUser(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || password == null)
            {
                return null;
            }

            var constraints = new Dictionary<Expression<Func<T, object>>, object> { { x => x.Login, login }, { x => x.Password, this.encryptionService.Encrypt(password) } };
            T membershipUser = this.membershipDao.FindOne(constraints);

            return membershipUser;
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="login">The user login.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        public void ChangePassword(string login, string oldPassword, string newPassword)
        {
            Check.Require<ArgumentNullException>(login != null, "Unable change password if user login is null");
            Check.Require<ArgumentException>(login != string.Empty, "Unable change password if user login is empty");
            Check.Require<ArgumentNullException>(oldPassword != null, "Unable change password if user old password is null");
            Check.Require<ArgumentNullException>(newPassword != null, "Unable change password if user new password is null");

            T foundUser = this.GetUser(login, oldPassword);
            if (foundUser != null)
            {
                foundUser.Password = this.encryptionService.Encrypt(newPassword);
                this.membershipDao.SaveOrUpdate(foundUser);
            }
        }
    }
}