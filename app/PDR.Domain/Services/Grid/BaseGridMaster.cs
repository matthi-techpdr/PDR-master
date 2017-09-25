using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;

using SmartArch.Data.Fetching;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Automapper;
using PDR.Domain.Services.Grid.Interfaces;
using SmartArch.Data;

namespace PDR.Domain.Services.Grid
{
    public abstract class BaseGridMaster<TEntity, TJsonModel, TViewModel> : IGridMaster<TEntity, TJsonModel, TViewModel>  
        where TEntity : Entity
        where TJsonModel : IJsonModel
        where TViewModel : class, IViewModel, new()
    {
        protected readonly IRepository<TEntity> repository;

        public IList<Expression<Func<TEntity, bool>>> ExpressionFilters { get; set; }

        protected BaseGridMaster(IRepository<TEntity> repository)
        {
            this.repository = repository;
            this.ExpressionFilters = new List<Expression<Func<TEntity, bool>>>();
        }

        public GridModel<TJsonModel> GetData(
            int page,
            int rows,
            string sortColumnName,
            string sort,
            IQueryable<TEntity> baseQuery = null,
            Expression<Func<TEntity, object>> orderExpression = null,
            Expression<Func<TEntity, object>> fetchFuction = null,
            long? additionalModelParam = -1,
            long? userId = null)
        {
            IQueryable<TEntity> query = baseQuery ?? this.repository.AsQueryable();

            var count = this.ExpressionFilters.Count;
            for (int i = 0; i < count; i++)
            {
                query = query.Where(this.ExpressionFilters[i]);
            }
            if (orderExpression == null)
            {
                if (!String.Equals(sortColumnName, "CarInfo") && !String.Equals(sortColumnName, "CarsMakeModelYear"))
                {
                    orderExpression = !sortColumnName.Contains("_")
                        ? GetOrderExpresion(sortColumnName == "Owner" ? "Employee" : sortColumnName == "InvoiceStatus" ? "Status" : sortColumnName)
                                             : GetCompositeOrderExpresion(sortColumnName);
                }
            }

            if (sortColumnName != "Role"
                && sortColumnName != "TotalAmount"
                && sortColumnName != "InvoiceAmount"
                && sortColumnName != "Owner"
                && sortColumnName != "CarInfo"
                && sortColumnName != "CarsMakeModelYear")
            {
                query = sort == "desc" ? query.OrderByDescending(orderExpression) : query.OrderBy(orderExpression);
            }
            
            if (fetchFuction != null)
            {
                query = query.Fetch(fetchFuction);
            }

            List<TEntity> entitiesView = null;

            switch (sortColumnName)
            {
                case "Role":
                    {
                        Func<TEntity, string> func = x => (x as User).Role.ToString();

                        if (typeof(TEntity) == typeof(License))
                        {
                            func = x => (x as License).Employee.Role.ToString();
                        }
                        
                        entitiesView = query.ToList();
                        entitiesView = sort == "desc" ? entitiesView.OrderByDescending(func).ToList() : entitiesView.OrderBy(func).ToList();
                        entitiesView = entitiesView.Skip((page - 1) * rows).Take(rows).ToList();
                    }

                    break;
                case "InvoiceAmount":
                case "TotalAmount":
                    {
                        Func<TEntity, double> func = x => x is Estimate ? (x as Estimate).TotalAmount : x is RepairOrder ? (x as RepairOrder).TotalAmount : (x as Invoice).TotalAmount;
                        entitiesView = query.ToList();
                        entitiesView = sort == "desc" ? entitiesView.OrderByDescending(func).ToList() : entitiesView.OrderBy(func).ToList();
                        entitiesView = entitiesView.Skip((page - 1) * rows).Take(rows).ToList();
                    }

                    break;
                case "CarInfo" : case "CarsMakeModelYear":
                    {
                        Func<TEntity, int> func1 = x => x is Estimate ? (x as Estimate).Car.Year : x is RepairOrder ? (x as RepairOrder).Estimate.Car.Year : (x as Invoice).RepairOrder.Estimate.Car.Year;
                        Func<TEntity, string> func2 = x => x is Estimate ? (x as Estimate).Car.Make : x is RepairOrder ? (x as RepairOrder).Estimate.Car.Make : (x as Invoice).RepairOrder.Estimate.Car.Make;
                        Func<TEntity, string> func3 = x => x is Estimate ? (x as Estimate).Car.Model : x is RepairOrder ? (x as RepairOrder).Estimate.Car.Model : (x as Invoice).RepairOrder.Estimate.Car.Model;
                        entitiesView = query.ToList();
                        entitiesView = sort == "desc" ? entitiesView.OrderByDescending(func1).ThenByDescending(func2).ThenByDescending(func3).ToList()
                                                        : entitiesView.OrderBy(func1).ThenBy(func2).ThenBy(func3).ToList();
                        entitiesView = entitiesView.Skip((page - 1) * rows).Take(rows).ToList();
                    }
                    break;

                case "Owner":
                    {
                        Func<TEntity, string> func = x => (x as License).Employee.Name;
                        entitiesView = query.ToList();
                        entitiesView = sort == "desc" ? entitiesView.OrderByDescending(func).ToList() : entitiesView.OrderBy(func).ToList();
                        entitiesView = entitiesView.Skip((page - 1) * rows).Take(rows).ToList();
                    }

                    break;
                default:
                    //if (userId == null)
                    //{
                    //    entitiesView = query.Skip((page - 1) * rows).Take(rows).ToList();
                    //    totalCount = query.Count();
                    //}
                    //else if (typeof(TEntity) == typeof(RepairOrder))
                    //{
                    //    var newSqlQuery = new SqlQueryForGrid<TEntity>();
                    //    entitiesView = newSqlQuery.GetRepairOrders(userId.Value, rows).ToList();
                    //    totalCount = newSqlQuery.CountRepairOrders(userId.Value);
                    //}
                   entitiesView = query.Skip((page - 1) * rows).Take(rows).ToList();
                   break;
            }

            if (!entitiesView.Any())
            {
                page = 1;
                entitiesView = query.Take(rows).ToList();
            }

            var totalCount = query.Count();
            var total = entitiesView.Count == 0 ? 1 : Math.Ceiling(totalCount / (float)rows);

            var gridModel = new GridModel<TJsonModel>
                {
                    page = page,
                    total = total,
                    records = totalCount,
                    rows = additionalModelParam == -1
                    ? entitiesView.Select(x => (TJsonModel)Activator.CreateInstance(typeof(TJsonModel), x)).ToList()
                    : entitiesView.Select(x => (TJsonModel)Activator.CreateInstance(typeof(TJsonModel), x, additionalModelParam)).ToList()
                };
            return gridModel;
        }

        public TViewModel GetEntityViewModel(long? id, Action<TEntity, TViewModel> entityCustomizer = null)
        {
           if (id == null)
            {
                return new TViewModel();
            }

            var entity = this.repository.Get(id.Value);
            var model = Mapper.DynamicMap<TEntity, TViewModel>(entity);
            if (entityCustomizer != null)
            {
                entityCustomizer.Invoke(entity, model);
            }
            
            return model;
        }

        public TEntity CreateEntity(TViewModel viewmodel, Action<TEntity> entityCustomizer = null)
        {
            var entity = CustomAutomapper<TViewModel, TEntity>.Map(viewmodel, entityCustomizer: entityCustomizer, isNewEntity:true);
            this.repository.Save(entity);
            return entity;
        }

        public void EditEntity(TViewModel viewmodel, Action<TEntity> entityCustomizer = null)
        {
            var currentEntity = this.repository.Get(long.Parse(viewmodel.Id));
            currentEntity = CustomAutomapper<TViewModel, TEntity>.Map(viewmodel, currentEntity, entityCustomizer);
            this.repository.Save(currentEntity);
        }

        public void SuspendEntity(long id)
        {
            var entity = this.repository.Get(id) as IEntityWithStatus;
            if (entity != null)
            {
                entity.Status = Statuses.Suspended;
                this.repository.Save((TEntity)entity);
            }
        }

        public void ActivateEntity(long id)
        {
            var entity = this.repository.Get(id) as IEntityWithStatus;
            if (entity != null)
            {
                entity.Status = Statuses.Active;
                this.repository.Save((TEntity)entity);
            }
        }

        private const string PARAMETER_NAME = "x";

        private static Expression<Func<TEntity, object>> GetOrderExpresion(string property)
        {
            ParameterExpression objExpr = Expression.Parameter(typeof(TEntity), PARAMETER_NAME);
            Expression accessExpr = Expression.Property(objExpr, property);
            accessExpr = Expression.Convert(accessExpr, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(accessExpr, PARAMETER_NAME, new[] { objExpr });
        }

        private static Expression<Func<TEntity, object>> GetCompositeOrderExpresion(string property)
        {
            var properties = property.Split('_');

            ParameterExpression objExpr = Expression.Parameter(typeof(TEntity), PARAMETER_NAME);
            Expression accessExpr = objExpr;
            foreach (var propertyName in properties)
            {
                accessExpr = Expression.Property(accessExpr, propertyName);
            }

            accessExpr = Expression.Convert(accessExpr, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(accessExpr, PARAMETER_NAME, new[] { objExpr });
        }
    }
}
