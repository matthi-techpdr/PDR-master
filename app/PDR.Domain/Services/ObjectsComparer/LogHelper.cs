using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Domain.Services.ObjectsComparer
{
    public class LogHelper 
    {
        public static string SplitPropertyName(string propertyName)
        {
            var output = string.Empty;
            foreach (var letter in propertyName)
            {
                if (char.IsUpper(letter))
                {
                    output += " " + letter;
                }
                else
                {
                    output += letter;
                }
            }

            return output.ToLower();
        }

        public string Compare<TViewModel>(TViewModel oldViewModel, TViewModel newViewModel, IList<string> exceptProperties = null)
            where TViewModel : IViewModel
        {
            var editedProperties = string.Empty;
            var type = typeof(TViewModel);
            var properties = type.GetProperties().ToList().Select(x => x.Name).ToList();

            if (exceptProperties != null)
            {
                properties = properties.Except(exceptProperties).ToList();
            }
            
            foreach (var propertyName in properties)
            {
                var oldVal = (oldViewModel.GetType().GetProperty(propertyName).GetValue(oldViewModel, null) ?? string.Empty).ToString();
                var newVal = (newViewModel.GetType().GetProperty(propertyName).GetValue(newViewModel, null) ?? string.Empty).ToString();
                if (oldVal != newVal)
                {
                    if (newVal == "True")
                    {
                        newVal = "+";
                    }

                    if (newVal == "False")
                    {
                        newVal = "-";
                    }

                    editedProperties += (string.Format("{0}: {1}; ", SplitPropertyName(propertyName), newVal));
                }
            }

            return editedProperties;
        }

        public T DeepClone<T>(T a)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public Dictionary<string, IList<long>> GetAddedAndRemovedFromCollectionById(IList<long> oldCollection, IList<long> newCollection)
        {
            var dictionary = new Dictionary<string, IList<long>>();
            var newItems = newCollection.Except(oldCollection).ToList();
            var removed = new List<long>();
            foreach (var item in oldCollection)
            {
                if (!newCollection.Contains(item))
                {
                    removed.Add(item);
                }
            }

            dictionary.Add("new", newItems);
            dictionary.Add("removed", removed);
            return dictionary;
        }
    }
}