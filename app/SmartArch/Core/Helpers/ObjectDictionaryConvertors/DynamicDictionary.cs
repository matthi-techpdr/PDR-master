using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace SmartArch.Core.Helpers.ObjectDictionaryConvertors
{
    /// <summary>
    /// Represents dynamic dictionary.
    /// </summary>
    public class DynamicDictionary : DynamicObject, IDictionary<string, object>
    {
        /// <summary>
        /// The core dictionary for storing members.
        /// </summary>
        private readonly IDictionary<string, object> dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary"/> class.
        /// </summary>
        public DynamicDictionary()
        {
            this.dictionary = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicDictionary"/> class.
        /// </summary>
        /// <param name="dictionary">The source dictionary.</param>
        public DynamicDictionary(IDictionary<string, object> dictionary)
        {
            this.dictionary = dictionary;
        }

        /// <summary>
        /// Provides the implementation for operations that get member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as getting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member on which the dynamic operation is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty) statement, where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="result">The result of the get operation. For example, if the method is called for a property, you can assign the property value to <paramref name="result"/>.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.dictionary.TryGetValue(binder.Name, out result);
        }

        /// <summary>
        /// Provides the implementation for operations that set member values. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations such as setting a value for a property.
        /// </summary>
        /// <param name="binder">Provides information about the object that called the dynamic operation. The binder.Name property provides the name of the member to which the value is being assigned. For example, for the statement sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies whether the member name is case-sensitive.</param>
        /// <param name="value">The value to set to the member. For example, for sampleObject.SampleProperty = "Test", where sampleObject is an instance of the class derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, the <paramref name="value"/> is "Test".</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.)
        /// </returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.dictionary[binder.Name] = value;

            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that set a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for operations that access objects by a specified index.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="indexes[0]"/> is equal to 3.</param>
        /// <param name="value">The value to set to the object that has the specified index. For example, for the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject is derived from the <see cref="T:System.Dynamic.DynamicObject"/> class, <paramref name="value"/> is equal to 10.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a language-specific run-time exception is thrown.
        /// </returns>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (this.dictionary.ContainsKey((string)indexes[0]))
            {
                this.dictionary[(string)indexes[0]] = value;
            }
            else
            {
                this.dictionary.Add((string)indexes[0], value);
            }

            return true;
        }

        /// <summary>
        /// Provides the implementation for operations that get a value by index. Classes derived from the <see cref="T:System.Dynamic.DynamicObject"/> class can override this method to specify dynamic behavior for indexing operations.
        /// </summary>
        /// <param name="binder">Provides information about the operation.</param>
        /// <param name="indexes">The indexes that are used in the operation. For example, for the sampleObject[3] operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived from the DynamicObject class, <paramref name="indexes[0]"/> is equal to 3.</param>
        /// <param name="result">The result of the index operation.</param>
        /// <returns>
        /// true if the operation is successful; otherwise, false. If this method returns false, the run-time binder of the language determines the behavior. (In most cases, a run-time exception is thrown.)
        /// </returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return this.dictionary.TryGetValue((string)indexes[0], out result);
        }

        #region Implementing IDictionary<string, object>        

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The dictionary enumerator.</returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return this.dictionary.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The new item.</param>
        public void Add(KeyValuePair<string, object> item)
        {
            this.dictionary.Add(item);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.dictionary.Clear();
        }

        /// <summary>
        /// Determines whether contains the specified item in dictionary.
        /// </summary>
        /// <param name="item">The contained item.</param>
        /// <returns>
        ///   <c>true</c> if contains the specified item in dictionary; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(KeyValuePair<string, object> item)
        {
            return this.dictionary.Contains(item);
        }

        /// <summary>
        /// Copies to array.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copy begins.</param>
        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            this.dictionary.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item which will removed.</param>
        /// <returns>true if success; otherwise false.</returns>
        public bool Remove(KeyValuePair<string, object> item)
        {
            return this.dictionary.Remove(item);
        }

        /// <summary>
        /// Gets the count of dictionary's items.
        /// </summary>
        public int Count
        {
            get
            {
                return this.dictionary.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is read only.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
        /// </value>
        public bool IsReadOnly
        {
            get
            {
                return this.dictionary.IsReadOnly;
            }
        }

        /// <summary>
        /// Determines whether the specified key contains key.
        /// </summary>
        /// <param name="key">The item key.</param>
        /// <returns>
        ///   <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return this.dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key of new item.</param>
        /// <param name="value">The value of new item.</param>
        public void Add(string key, object value)
        {
            this.dictionary.Add(key, value);
        }

        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key of item which will removed.</param>
        /// <returns>true if success; otherwise false.</returns>
        public bool Remove(string key)
        {
            return this.dictionary.Remove(key);
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key">The item's key.</param>
        /// <param name="value">The item's value.</param>
        /// <returns>true if success; otherwise false.</returns>
        public bool TryGetValue(string key, out object value)
        {
            return this.dictionary.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <param name="key">The string index key.</param>        
        /// <returns>The value by string index.</returns>
        public object this[string key]
        {
            get
            {
                return this.dictionary[key];
            }

            set
            {
                this.dictionary[key] = value;
            }
        }

        /// <summary>
        /// Gets the all keys of dictionary.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return this.dictionary.Keys;
            }
        }

        /// <summary>
        /// Gets the all values of dictionary.
        /// </summary>
        public ICollection<object> Values
        {
            get
            {
                return this.dictionary.Values;
            }
        }

        #endregion
    }
}