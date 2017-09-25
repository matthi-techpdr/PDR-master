using System;

namespace PDR.Domain.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class AverageSizeStringAttribute : Attribute
    {
        public AverageSizeStringAttribute(string name)
        {
            this.Name = name;
        }
        
        public string Name { get; set; }
    }
}