using System;
using FluentValidation;
using StructureMap;

namespace PDR.Init.FluentValidation
{
    public class StructureMapValidatorFactory : ValidatorFactoryBase
    {
        public override IValidator CreateInstance(Type validatorType)
        {
            return ObjectFactory.TryGetInstance(validatorType) as IValidator;
        }
    }
}
