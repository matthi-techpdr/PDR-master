using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using SmartArch.Data;

namespace PDR.Domain.RepositoryExtenssions
{
    public static class RepositoryExtenssions
    {
        public static IQueryable<T> OnlyActive<T>(this IRepository<T> entities) where T : IEntityWithStatus
        {
            return entities.Where(x => x.Status == Statuses.Active);
        }

        public static IList<T> OnlyActive<T>(this IList<T> entities) where T : IEntityWithStatus
        {
            return entities.Where(x => x.Status == Statuses.Active).ToList();
        }
    }
}