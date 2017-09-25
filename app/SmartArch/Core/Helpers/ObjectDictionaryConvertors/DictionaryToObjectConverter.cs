using System.Collections.Generic;
using System.Linq;

namespace SmartArch.Core.Helpers.ObjectDictionaryConvertors
{
    /// <summary>
    /// Loads the values of a <see cref="IDictionary{String,Object}"/> into object.
    /// </summary>
    public static class DictionaryToObjectConverter
    {
        /// <summary>
        /// Converts dictionary to anonimouse object by prototype.
        /// </summary>
        /// <see cref="http://jacobcarpenter.wordpress.com/2008/03/13/dictionary-to-anonymous-type/"/>
        /// <example>
        ///    var dict = new Dictionary{string, object} {
        ///    { "Name", "Jacob" },
        ///    { "Age", 26 },
        ///    { "FavoriteColors", new[] { ConsoleColor.Blue, ConsoleColor.Green } },
        ///    };
        ///    var person = dict.ToAnonymousType(
        ///     new
        ///    {
        ///    Name = default(string),
        ///    Age = default(int),
        ///    FavoriteColors = default(IEnumerable{ConsoleColor}),
        ///    Birthday = default(DateTime?),
        ///     });
        /// </example>
        /// <param name="dictionary">The source dictionary.</param>
        /// <param name="anonymousPrototype">The prototype of anonymouse object.</param>
        /// <returns>The anonymouse object.</returns>
        public static object ToAnonymous(this IDictionary<string, object> dictionary, object anonymousPrototype)
        {
            // get the sole constructor
            var ctor = anonymousPrototype.GetType().GetConstructors().Single();
            // conveniently named constructor parameters make this all possible...
            var args = from p in ctor.GetParameters()
                       let val = dictionary.GetValueOrDefault(p.Name)
                       select val != null && p.ParameterType.IsAssignableFrom(val.GetType()) ? val : null;

            return ctor.Invoke(args.ToArray());
        }

        /// <summary>
        /// Converts the specified dictionary to anonymouse object.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns>The anonymouse object.</returns>
        public static dynamic ToAnonymous(this IDictionary<string, object> dictionary)
        {
            dynamic dynamicDictionary = new DynamicDictionary(dictionary);
            
            return dynamicDictionary;
        }

        /// <summary>
        /// Gets the value or default from dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The item's key.</param>
        /// <returns>The value or default.</returns>
        private static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue result;
            dictionary.TryGetValue(key, out result);

            return result;
        }
    }
}