using PDR.Domain.Model.Users;

namespace PDR.Domain.Services.Account
{
    /// <summary>
    /// Represents result of authorization.
    /// </summary>
    public class AuthorizationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationResult"/> class.
        /// </summary>
        public AuthorizationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationResult"/> class.
        /// Sets default message.
        /// </summary>
        /// <param name="success">if set to <c>true</c> that means authorization is success.</param>
        public AuthorizationResult(bool success)
        {
            this.Success = success;
            this.Message = success ? string.Empty : "Incorrect login or password";
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AuthorizationResult"/> is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message about result of authorization.
        /// </summary>
        /// <value>
        /// The authorization message.
        /// </value>
        public string Message { get; set; }

        public User User { get; set; }
    }
}