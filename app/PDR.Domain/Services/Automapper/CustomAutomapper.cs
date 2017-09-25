using System;
using AutoMapper;

namespace PDR.Domain.Services.Automapper
{
    public static class CustomAutomapper<TSource, TDestination>
        where TSource : class
        where TDestination : class
    {
        public static TDestination Map(TSource inInstance, TDestination outInstance = null, Action<TDestination> entityCustomizer = null, bool? isNewEntity = null)
        {
            return CustomAutomapper.Map(inInstance, outInstance, entityCustomizer, isNewEntity);
        }
    }

    public static class CustomAutomapper
    {
        public static TDestination Map<TSource, TDestination>(TSource inInstance, TDestination outInstance = null, Action<TDestination> entityCustomizer = null, bool? isNewEntity = null)
            where TSource : class
            where TDestination : class
        {
            if (outInstance == null)
            {
                if (isNewEntity.HasValue)
                {
                    outInstance = (TDestination)Activator.CreateInstance(typeof(TDestination), isNewEntity);
                }
                else
                {
                    outInstance = Activator.CreateInstance<TDestination>();
                }
            }

            Mapper.DynamicMap(inInstance, outInstance);

            if (entityCustomizer != null)
            {
                entityCustomizer.Invoke(outInstance);
            }

            return outInstance;
        }
    }
}
