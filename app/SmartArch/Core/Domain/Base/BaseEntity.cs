namespace SmartArch.Core.Domain.Base
{
    /// <summary>
    /// Base functionality for all entities.
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// The multiplier for calculation hash code.
        /// </summary>
        private const int HASH_MULTIPLIER = 31;

        /// <summary>
        /// The cached hash code of entity.
        /// </summary>
        private int? cachedHashCode;

        /// <summary>
        /// Gets the entity id.
        /// </summary>
        /// <value>The entity id value.</value>
        public virtual long Id { get; protected set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            BaseEntity entityWithTypedId = obj as BaseEntity;
            bool isEqual = ReferenceEquals(this, entityWithTypedId) ||
                           (entityWithTypedId != null && (this.HasSameNonDefaultIdAs(entityWithTypedId) || (this.IsTransient() && entityWithTypedId.IsTransient())));

            return isEqual;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int value;
            if (this.cachedHashCode.HasValue)
            {
                value = this.cachedHashCode.Value;
            }
            else
            {
                if (this.IsTransient())
                {
                    this.cachedHashCode = new int?(base.GetHashCode());
                }
                else
                {
                    int hashCode = this.GetType().GetHashCode();
                    int multipliedHashCode = hashCode * HASH_MULTIPLIER;
                    long id = this.Id;
                    this.cachedHashCode = new int?(multipliedHashCode ^ id.GetHashCode());
                }

                value = this.cachedHashCode.Value;
            }

            return value;
        }

        /// <summary>
        /// Determines whether this instance is transient.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is transient; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsTransient()
        {
            bool isTransient = this.Id.Equals(default(long));

            return isTransient;
        }

        /// <summary>
        /// Determines whether [has same non default id as] [the specified compare to].
        /// </summary>
        /// <param name="compareTo">The compare to.</param>
        /// <returns>
        /// 	<c>true</c> if [has same non default id as] [the specified compare to]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasSameNonDefaultIdAs(BaseEntity compareTo)
        {
            bool hasSameNonDefaultId;
            if (!this.IsTransient() && !compareTo.IsTransient())
            {
                hasSameNonDefaultId = this.Id.Equals(compareTo.Id);
            }
            else
            {
                hasSameNonDefaultId = false;
            }

            return hasSameNonDefaultId;
        }
    }
}