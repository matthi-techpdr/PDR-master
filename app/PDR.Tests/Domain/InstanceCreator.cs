
namespace PDR.Tests.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using PDR.Domain.Model.Base;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class InstanceCreator
    {
        static InstanceCreator()
        {
            EntitiesTypes =
                Assembly.GetAssembly(typeof(Entity)).GetTypes().Where(
                    t => t.IsSubclassOf(typeof(Entity)) && !t.IsAbstract);
            InstanceCollection = new Dictionary<Type, object>();

            InstanceCollection[typeof(int)] = 50;
            InstanceCollection[typeof(float)] = 20.1;
            InstanceCollection[typeof(double)] = 200.3;
            InstanceCollection[typeof(string)] = "teststring";
            InstanceCollection[typeof(DateTime)] = DateTime.Now;
            InstanceCollection[typeof(bool)] = true;
            InstanceCollection[typeof(byte[])] = new byte[2];
        }

        public static IEnumerable<Type> EntitiesTypes { get; set; }

        private static Dictionary<Type, object> InstanceCollection { get; set; }

        private object GetPropertyValue(Type propertyType)
        {
            if (EntitiesTypes.Contains(propertyType))
            {
                return this.CreateSimpleInstance(propertyType);
            }

            if (InstanceCollection.ContainsKey(propertyType))
            {
                return InstanceCollection[propertyType];
            }

            return null;
        }

        public object CreateSimpleInstance(Type type, object reference = null)
        {
           var instance = Activator.CreateInstance(type);
           var properties = type.GetProperties().Where(p => p.CanWrite && p.CanRead && p.PropertyType.IsSubclassOf(typeof(Entity)) || InstanceCollection.Keys.Contains(p.PropertyType));
           foreach (var p in properties)
           {
               object value = null;
               if (p.PropertyType.IsSubclassOf(typeof(Entity)))
               {
                   var targetPropertyType = p.PropertyType;
                   if (p.PropertyType.IsAbstract)
                   {
                       targetPropertyType = EntitiesTypes.First(t => t.IsSubclassOf(targetPropertyType));
                   }

                   if (reference != null && reference.GetType() == targetPropertyType)
                   {
                       value = reference;
                   }
                   else
                   {
                       value = this.CreateSimpleInstance(targetPropertyType, instance);
                   }
               }

               if (InstanceCollection.ContainsKey(p.PropertyType))
               {
                   value = InstanceCollection[p.PropertyType];
               }

               p.SetValue(instance, value, null);
           }

           return instance;
        }

        public void SetReferences(object entity)
        {
            var properties = entity.GetType().GetProperties().Where(p => p.CanWrite && p.CanRead);
            foreach (var p in properties)
            {
                var value = this.GetPropertyValue(p.PropertyType);
                p.SetValue(entity, value, null);
            }
        }

        public IEnumerable<object> GetInstances()
        {
            return EntitiesTypes.Select(x => this.CreateSimpleInstance(x));
        }
    }
}
