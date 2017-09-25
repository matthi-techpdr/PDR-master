﻿using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Core.Attributes;
using PDR.Web.WebAPI.Authorization;

namespace PDR.Web.WebAPI
{
    using PDR.Domain.Model.Enums;

    [ApiAuthorize]
    public class EstimateToRepairOrderController : ApiController
    {
        private static ReassignHelper ReassignHelper
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReassignHelper>();
            }
        }

        private static Employee Employee
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
            }
        }

        public HttpResponseMessage Get(long id, bool forConvert)
        {
            var estimateId = new long[1];
            estimateId[0] = id;
            var employees = ReassignHelper.GetReassignEmployees(Employee, estimateId, true);
            if (employees == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }

            var managersList = new List<dynamic>();
            foreach (var m in employees)
            {
                if(forConvert && m.Role == UserRoles.RITechnician){continue;}
                dynamic model = new ExpandoObject();
                model.Name = m.Name;
                model.Id = m.Id;
                model.IsRiTechnician = m.Role == UserRoles.RITechnician;
                managersList.Add(model);
            }

            return Request.CreateResponse(HttpStatusCode.OK, managersList);
        }

        [WebApiTransaction]
        public HttpResponseMessage Post(long managerId, long estimateId)
        {
            var successs = ReassignHelper.ConvertToRepairOrder(estimateId, managerId, Employee);
            return new HttpResponseMessage(successs ? HttpStatusCode.Created : HttpStatusCode.Conflict);
        }
    }
}