using System;

namespace PDR.Domain.Model.Enums.Attributes
{
    public class DescriptionAttribute: Attribute
    {
        private string description;

        public DescriptionAttribute(string value)
        {
            description = value;
        }
        public string Description {
            get
            {
                return description;
            }
        }
    }
}
