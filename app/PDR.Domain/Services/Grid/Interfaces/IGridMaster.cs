using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

namespace PDR.Domain.Services.Grid.Interfaces
{
    public interface IGridMaster<TEntity, TJsonModel, TViewModel>
    {
        IList<Expression<Func<TEntity, bool>>> ExpressionFilters { get; set; }

        void SuspendEntity(long id);

        void ActivateEntity(long id);

        GridModel<TJsonModel> GetData(
            int page,
            int rows,
            string sortColumnName,
            string sort,
            IQueryable<TEntity> baseQuery = null,
            Expression<Func<TEntity, object>> orderExpression = null,
            Expression<Func<TEntity, object>> fetchFuction = null,
            long? additionalModelParam = -1,
            long? userId = null);

        TViewModel GetEntityViewModel(long? id, Action<TEntity, TViewModel> entityCustomizer = null);

        TEntity CreateEntity(TViewModel viewmodel, Action<TEntity> entityCustomizer = null);

        void EditEntity(TViewModel viewmodel, Action<TEntity> entityCustomizer = null);
    }
}
