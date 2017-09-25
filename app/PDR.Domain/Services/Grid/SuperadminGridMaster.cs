using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Domain.Services.Grid
{
    public class SuperadminGridMaster<TEntity, TJsonModel, TViewModel> : BaseGridMaster<TEntity, TJsonModel, TViewModel>,
        ISuperadminGridMaster<TEntity, TJsonModel, TViewModel>
        where TEntity : Entity
        where TJsonModel : IJsonModel
        where TViewModel : class, IViewModel, new()
    {
        public SuperadminGridMaster(INativeRepository<TEntity> repository)
            : base(repository)
        {
        }
    }
}
