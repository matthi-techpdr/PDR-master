using System;
using System.Web.Mvc;

namespace PDR.Web.Core.Formatters
{
    public class CustomModelBinder : DefaultModelBinder
    {
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            var propertyType = propertyDescriptor.PropertyType;
            if (propertyType.IsEnum)
            {
                var providerValue = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
                if (null != providerValue)
                {
                    var value = providerValue.RawValue;
                    if (null != value)
                    {
                        var valueType = value.GetType();
                        if (!valueType.IsEnum)
                        {
                            return Enum.ToObject(propertyType, value);
                        }
                    }
                }
            }
            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }
}