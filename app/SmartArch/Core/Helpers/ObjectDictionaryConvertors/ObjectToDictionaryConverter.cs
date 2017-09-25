using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace SmartArch.Core.Helpers.ObjectDictionaryConvertors
{
    /// <summary>
    /// Loads the values of an object's properties into a <see cref="IDictionary{String,Object}"/>
    /// </summary>
    public static class ObjectToDictionaryConverter
    {
        /// <summary>
        /// The converters cache.
        /// </summary>
        private static readonly Dictionary<Type, Func<object, IDictionary<string, object>>> Cache = new Dictionary<Type, Func<object, IDictionary<string, object>>>();

        /// <summary>
        /// The reader-writer lock slim.
        /// </summary>
        private static readonly ReaderWriterLockSlim ReaderWriterLockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// Loads the values of an object's properties into a <see cref="IDictionary{String,Object}"/>.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <returns>If <paramref name="dataObject"/> implements <see cref="IDictionary{String,Object}"/>, 
        /// the object is cast to <see cref="IDictionary{String,Object}"/> and returned.
        /// Otherwise the object returned is a <see cref="System.Collections.Hashtable"/> with all public non-static properties and their respective values
        /// as key-value pairs.
        /// </returns>
        public static IDictionary<string, object> ToDictionary(this object dataObject)
        {
            if (dataObject == null)
            {
                return null;
            }

            if (dataObject is IDictionary<string, object>)
            {
                return (IDictionary<string, object>)dataObject;
            }

            return GetObjectToDictionaryConverter(dataObject)(dataObject);
        }

        /// <summary>
        /// Handles caching for getting convertor.
        /// </summary>
        /// <param name="item">The source item.</param>
        /// <returns>The function for convert object to dictionary.</returns>
        private static Func<object, IDictionary<string, object>> GetObjectToDictionaryConverter(object item)
        {
            ReaderWriterLockSlim.EnterUpgradeableReadLock();
            try
            {
                Func<object, IDictionary<string, object>> ft;
                if (!Cache.TryGetValue(item.GetType(), out ft))
                {
                    ReaderWriterLockSlim.EnterWriteLock();
                    // double check
                    try
                    {
                        if (!Cache.TryGetValue(item.GetType(), out ft))
                        {
                            ft = CreateObjectToDictionaryConverter(item.GetType());
                            Cache[item.GetType()] = ft;
                        }
                    }
                    finally
                    {
                        ReaderWriterLockSlim.ExitWriteLock();
                    }
                }

                return ft;
            }
            finally
            {
                ReaderWriterLockSlim.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Creates the object to dictionary converter.
        /// </summary>
        /// <param name="itemType">Type of the item.</param>
        /// <returns>The function for convert object to dictionary.</returns>
        private static Func<object, IDictionary<string, object>> CreateObjectToDictionaryConverter(Type itemType)
        {
            var dictType = typeof(Dictionary<string, object>);

            // setup dynamic method
            // Important: make itemType owner of the method to allow access to internal types
            var dm = new DynamicMethod(string.Empty, typeof(IDictionary<string, object>), new[] { typeof(object) }, itemType);
            var il = dm.GetILGenerator();

            // Dictionary.Add(object key, object value)
            var addMethod = dictType.GetMethod("Add");

            // create the Dictionary and store it in a local variable
            il.DeclareLocal(dictType);
            il.Emit(OpCodes.Newobj, dictType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);

            const BindingFlags ATTRIBUTES = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            foreach (var property in itemType.GetProperties(ATTRIBUTES).Where(info => info.CanRead))
            {
                // load Dictionary (prepare for call later)
                il.Emit(OpCodes.Ldloc_0);
                // load key, i.e. name of the property
                il.Emit(OpCodes.Ldstr, property.Name);

                // load value of property to stack
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Callvirt, property.GetGetMethod(), null);
                // perform boxing if necessary
                if (property.PropertyType.IsValueType)
                {
                    il.Emit(OpCodes.Box, property.PropertyType);
                }

                // stack at this point
                // 1. string or null (value)
                // 2. string (key)
                // 3. dictionary

                // ready to call dict.Add(key, value)
                il.EmitCall(OpCodes.Callvirt, addMethod, null);
            }
            // finally load Dictionary and return
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return (Func<object, IDictionary<string, object>>)dm.CreateDelegate(typeof(Func<object, IDictionary<string, object>>));
        }
    }
}