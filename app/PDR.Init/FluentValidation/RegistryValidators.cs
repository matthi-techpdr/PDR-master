using FluentValidation;
using PDR.Domain.StaticCollections;
using StructureMap.Configuration.DSL;

namespace PDR.Init.FluentValidation
{
    public class RegistryValidators : Registry
    {
        public RegistryValidators()
        {
            foreach (var assembly in ProjectsAssemblies.GetAssemblies())
            {
                AssemblyScanner.FindValidatorsInAssembly(assembly)
                .ForEach(result => this.For(result.InterfaceType)
                                       .Singleton()
                                       .Use(result.ValidatorType));
            }
        }

        public RegistryValidators(System.Reflection.Assembly assembly)
        {
                AssemblyScanner.FindValidatorsInAssembly(assembly)
                .ForEach(result => this.For(result.InterfaceType)
                                       .Singleton()
                                       .Use(result.ValidatorType));
        }

    }
}
