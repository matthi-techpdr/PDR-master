using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using AutoMapper;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

using PDR.Web.Core.Attributes;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;

using SmartArch.Data;

namespace PDR.Web.WebAPI
{
    [ApiAuthorize]
    public abstract class BaseWebApiController<TEntity, TModel> : ApiController
        where TEntity : Entity, ICompanyEntity where TModel : BaseIPhoneModel
    {
        private ICompanyRepository<TEntity> repository;
        
        protected ICompanyRepository<TEntity> Repository
        {
            get
            {
                return this.repository ?? (this.repository = ServiceLocator.Current.GetInstance<ICompanyRepository<TEntity>>());  
            }
        }

        public virtual IList<TModel> Get()
        {
            var selectCommand = this.Request.RequestUri.ParseQueryString()["select"];
            var entities = this.Repository.ToList();

            var query = entities.AsQueryable();
            
            if (typeof(TEntity) is IEntityWithStatus)
            {
                query = query.Where(x => ((IEntityWithStatus)x).Status == Statuses.Active);
            }

            var model = query.Select(e => (TModel)Activator.CreateInstance(typeof(TModel), e)).ToList().AsQueryable();

            if (selectCommand != null)
            {
                var properties = selectCommand.Split(',');
                return model.Select(m => this.GetModelWithSelectedProperties(m, properties)).ToList();
            }

            return model.ToList();
        }

        public virtual HttpResponseMessage Get(long id)
        {
            var entity = this.Repository.Get(id);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = (TModel)Activator.CreateInstance(typeof(TModel), entity);

            var selectCommand = this.Request.RequestUri.ParseQueryString()["select"];

            if (selectCommand == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, model);
            }

            var properties = selectCommand.Split(',');
            TModel obj = GetModelWithSelectedProperties(model, properties);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }
      
        [WebApiTransaction]
        public virtual HttpResponseMessage Post(TModel model)
        {
            if (model == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Conflict));
            }

            var entity = Mapper.DynamicMap<TEntity>(model);
            this.Repository.Save(entity);
            var response = new HttpResponseMessage(HttpStatusCode.Created);
            response.Headers.Location = new Uri(new Uri(Request.RequestUri.AbsoluteUri.Split('?')[0]), entity.Id.ToString());
            return response;
        }

        [WebApiTransaction]
        public virtual void Put(TModel entity)
        {
            if (entity.Id == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            
            var currentEntity = this.Repository.Get(entity.Id.Value);
            if (currentEntity == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            Mapper.DynamicMap(entity, currentEntity);
            this.Repository.Save(currentEntity);
        }

        protected TModel GetModelWithSelectedProperties(TModel model, IEnumerable<string> requestProperties)
        {
            var modelProperties = typeof(TModel).GetProperties();
            foreach (var modelProperty in modelProperties.Where(modelProperty => !requestProperties.Contains(modelProperty.Name)))
            {
                modelProperty.SetValue(model, null, null);
            }
            return model;
        }
    }
}
