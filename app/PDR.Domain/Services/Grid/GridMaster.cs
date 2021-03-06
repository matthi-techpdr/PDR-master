﻿using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Domain.Services.Grid
{
    public class GridMaster<TEntity, TJsonModel, TViewModel> : BaseGridMaster<TEntity, TJsonModel, TViewModel>
        where TEntity : Entity, ICompanyEntity
        where TJsonModel : IJsonModel
        where TViewModel : class, IViewModel, new()
    {
        public GridMaster(ICompanyRepository<TEntity> repository)
            : base(repository)
        {
        }
    }
}
