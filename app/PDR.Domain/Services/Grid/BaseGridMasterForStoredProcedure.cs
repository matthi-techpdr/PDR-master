using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Automapper;
using PDR.Domain.Services.Grid.Interfaces;
using SmartArch.Data;
using PDR.Domain.StoredProcedureHelpers;

namespace PDR.Domain.Services.Grid
{
    using System.Web.Mvc;
    using System.Web;

    using PDR.Domain.Helpers;
    using PDR.Domain.Model.Customers;

    public abstract class BaseGridMasterForStoredProcedure<TEntity, TJsonModel, TViewModel> : IGridMasterForStoredProcedure<TEntity, TJsonModel, TViewModel>  
        where TEntity : Entity
        where TJsonModel : IJsonModel
        where TViewModel : class, IViewModel, new()
    {
        protected readonly IRepository<TEntity> repository;

        private long? CustomerId { get; set; }

        protected BaseGridMasterForStoredProcedure(IRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        public GridModel<TJsonModel> GetData(IStoredProcedureHelper storedProcedureHelper, int rows, int page, long? additionalModelParam = -1, bool isCustomerFilter = false, HttpCookie customerCookie = null)
        {
            List<TEntity> entitiesView = null;
            var customers = new List<Customer>();
            int totalCount = 0;
            if (storedProcedureHelper is InvoicesStoredProcedureHelper)
            {
                var helper = storedProcedureHelper as InvoicesStoredProcedureHelper;
                entitiesView = (List<TEntity>)helper.Invoices;
                if (entitiesView != null && !entitiesView.Any())
                {
                    entitiesView = (List<TEntity>)helper.InvoicesFirstPage;
                    page = 1;
                }
                totalCount = helper.TotalCountRows;
                if (isCustomerFilter)
                {
                    customers = helper.GetCustomersForFilter().ToList();
                }
            }

            if (storedProcedureHelper is RepairOrdersStoredProcedureHelper)
            {
                var helper = storedProcedureHelper as RepairOrdersStoredProcedureHelper;
                entitiesView = (List<TEntity>)helper.RepairOrders;
                if (entitiesView != null && !entitiesView.Any())
                {
                    entitiesView = (List<TEntity>)helper.RepairOrdersFirstPage;
                    page = 1;
                }
                totalCount = helper.TotalCountRows;
                if (isCustomerFilter)
                {
                    customers = helper.GetCustomersForFilter().ToList();
                }
            }

            if (storedProcedureHelper is EstimatesStoredProcedureHelper)
            {
                var helper = storedProcedureHelper as EstimatesStoredProcedureHelper;
                entitiesView = (List<TEntity>)helper.Estimates;
                if (entitiesView != null && !entitiesView.Any())
                {
                    entitiesView = (List<TEntity>)helper.EstimatesFirstPage;
                    page = 1;
                }
                totalCount = helper.TotalCountRows;
                if (isCustomerFilter)
                {
                    customers = helper.GetCustomersForFilter().ToList();
                }
            }


            var total = entitiesView != null && entitiesView.Count == 0 ? 1 : Math.Ceiling(totalCount / (float)rows);

            var customersFilter = new List<SelectListItem>();
            if (customers.Any())
            {
                var customer = customerCookie != null ? customerCookie.Value : null;
                long customerId = 0;
                var resultTryParseCustomer = Int64.TryParse(customer, out customerId);
                CustomerId = resultTryParseCustomer ? customerId : (long?)null;

                customersFilter = GetCustomerSelectedList(customers).ToList();
            }

            var gridModel = new GridModel<TJsonModel>
                {
                    page = page,
                    total = total,
                    records = totalCount,
                    rows = additionalModelParam == -1
                        ? entitiesView.Select(x => (TJsonModel)Activator.CreateInstance(typeof(TJsonModel), x)).ToList()
                        : entitiesView.Select(x => (TJsonModel)Activator.CreateInstance(typeof(TJsonModel), x, additionalModelParam)).ToList(),
                    customersFilter = customersFilter.Any()
                        ? customersFilter
                        : null
                };
            return gridModel;
        }

        private IList<SelectListItem> GetCustomerSelectedList(IEnumerable<Customer> customers)
        {
            var selectedListTmp = customers.Select(x => new SelectListItem
            {
                Text = x.GetCustomerName(),
                Value = x.Id.ToString(),
                Selected = CustomerId.HasValue && x.Id == CustomerId.Value
            });
            var selectedList = selectedListTmp.OrderBy(x => x.Text).ToList();
            if (CustomerId == null)
            {
                selectedList.Insert(0, new SelectListItem { Text = @"All customers", Selected = true, Value = null });
            }
            else if (selectedList.Any(x => x.Selected))
            {
                var item = selectedList.Single(x => x.Selected);
                selectedList.Remove(item);
                selectedList.Insert(0, item);
                selectedList.Insert(1, new SelectListItem { Text = @"All customers", Selected = false, Value = null });
            }
            else
            {
                selectedList.Insert(0, new SelectListItem { Text = @"All customers", Selected = true, Value = null });
            }

            return selectedList;
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
    }
}
