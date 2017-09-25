using SmartArch.Data;

namespace PDR.Domain.Services.Account
{
    using System;
    using System.Linq;
    using PDR.Domain.Contracts.Repositories;
    using PDR.Domain.Model.Enums;
    using PDR.Domain.Model.Users;

    public class RolesProvider : IRolesProvider
    {
        /// <summary>
        /// User repository.
        /// </summary>
        private readonly INativeRepository<User> userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RolesProvider"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        public RolesProvider(INativeRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Get user roles.
        /// </summary>
        /// <returns>
        /// User roles.
        /// </returns>
        public string[] GetUserRoles(long userId)
        {
            var user = this.userRepository.Get(userId);
            
            if (user != null)
            {
                var result = (from UserRoles r in Enum.GetValues(typeof(UserRoles)) where user.Role.HasFlag(r) select r.ToString()).ToArray();
                return result;
            }

            return new string[0];
        }
    }
}
