using System.Linq;
using SmartArch.Core.Domain.Base;
using SmartArch.Data.Specification;

namespace SmartArch.Data
{
    public static class RepositoryExtensions
    {
         public static TEntity Get<TEntity>(this IRepository<TEntity> repository, long id) where TEntity : BaseEntity
         {
             TEntity entity = repository.SingleOrDefault(x => x.Id == id);

             return entity;
         }

         public static void Remove<TEntity>(this IRepository<TEntity> repository, long id) where TEntity : BaseEntity
         {
             TEntity entity = repository.SingleOrDefault(x => x.Id == id);
             repository.Remove(entity);
         }

        public static Finder<TEntity> Find<TEntity>(this IRepository<TEntity> repository) where TEntity : BaseEntity
        {
            var finder = new Finder<TEntity>(repository);
            return finder;
        }
    }
}