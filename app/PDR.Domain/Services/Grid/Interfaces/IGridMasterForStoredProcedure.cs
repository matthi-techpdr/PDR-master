using System;
using System.Linq;
using PDR.Domain.StoredProcedureHelpers;
using System.Web;

namespace PDR.Domain.Services.Grid.Interfaces
{
    public interface IGridMasterForStoredProcedure<TEntity, TJsonModel, TViewModel>
    {
        void SuspendEntity(long id);

        void ActivateEntity(long id);

        GridModel<TJsonModel> GetData(IStoredProcedureHelper storedProcedureHelper, int rows, int page, long? additionalModelParam = -1, bool isCustomerFilter = false, HttpCookie customerCookie = null);

        TViewModel GetEntityViewModel(long? id, Action<TEntity, TViewModel> entityCustomizer = null);

        TEntity CreateEntity(TViewModel viewmodel, Action<TEntity> entityCustomizer = null);

        void EditEntity(TViewModel viewmodel, Action<TEntity> entityCustomizer = null);
    }
}
