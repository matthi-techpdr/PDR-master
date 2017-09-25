using NHibernate;

namespace SmartArch.Data.NH
{
    /// <summary>
    /// Represents class for managing <c>NHibernate</c> transaction.
    /// </summary>
    public class TransactionManager : ITransactionManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionManager"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public TransactionManager(ISession session)
        {
            this.Session = session;
        }

        /// <summary>
        /// The <c>NHibernate</c> session.
        /// </summary>
        public ISession Session { get; set; }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        protected ITransaction Transaction
        {
            get
            {
                return this.Session.Transaction;
            }
        }

        /// <summary>
        /// Begins transaction.
        /// </summary>
        public void Begin()
        {
            this.Transaction.Begin();
        }

        /// <summary>
        /// Commits transaction.
        /// </summary>
        public void Commit()
        {
            this.Transaction.Commit();
        }

        /// <summary>
        /// Rollbacks transaction.
        /// </summary>
        public void Rollback()
        {
            this.Transaction.Rollback();
        }

        /// <summary>
        /// Gets a value indicating whether current transaction is active.
        /// </summary>
        /// <value>
        /// <c>true</c> if current transaction is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get
            {
                return this.Transaction.IsActive;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Transaction.Dispose();
        }
    }
}