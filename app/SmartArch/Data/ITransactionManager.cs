using System;

namespace SmartArch.Data
{
    /// <summary>
    /// Represents interface for managing transaction.
    /// </summary>
    public interface ITransactionManager : IDisposable
    {
        /// <summary>
        /// Begins transaction.
        /// </summary>
        void Begin();

        /// <summary>
        /// Commits transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollbacks transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Gets a value indicating whether current transaction is active.
        /// </summary>
        /// <value><c>true</c> if current transaction is active; otherwise, <c>false</c>.</value>
        bool IsActive { get; }
    }
}