using System.Linq;

namespace SmartArch.Data.Fetching
{
    /// <summary>
    /// Represents fetch request interface.
    /// </summary>
    /// <typeparam name="TQueried">The type of the queried.</typeparam>
    /// <typeparam name="TFetch">The type of the fetch.</typeparam>
    public interface IFetchRequest<TQueried, TFetch> : IOrderedQueryable<TQueried>
    {
    }
}