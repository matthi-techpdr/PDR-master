using System.Collections.Generic;
using System.Linq;

using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

using PDR.Web.Areas.Estimator.Models.Estimates;

using SmartArch.Data;
using SmartArch.Data.Proxy;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Common.Models
{
    using System.Web.Mvc;

    using PDR.Web.Core.Helpers;

    public class CustomersResolver
    {
        private readonly IRepository<WholesaleCustomer> wholesaleCustomersRepository;

        private readonly IRepository<Affiliate> affiliatesRepository;

        private readonly ICurrentWebStorage<Employee> userStorage;

        public CustomersResolver(IRepositoryFactory repositoryFactory, ICurrentWebStorage<Employee> userStorage)
        {
            this.wholesaleCustomersRepository = repositoryFactory.CreateForCompany<WholesaleCustomer>();
            this.affiliatesRepository = repositoryFactory.CreateForCompany<Affiliate>();
            this.userStorage = userStorage;
        }

        public EstimateModel CustomerResolve(Estimate estimate = null, EstimateModel estimateModel = null, string type = "")
        {
            var currentUser = this.userStorage.Get();
            var customers = (currentUser is Domain.Model.Users.Technician || currentUser is Domain.Model.Users.Manager)
                                ? ((TeamEmployee)currentUser).Teams.Where(x => x.Status == Statuses.Active).SelectMany(x => x.Customers).Distinct()
                                    .Select(x => x.ToPersist<WholesaleCustomer>()).Where(x => x != null).Where(x => x.Status == Statuses.Active).ToList()
                               : this.wholesaleCustomersRepository.Where(x => x.Status == Statuses.Active).ToList();

            var affilates = (currentUser is Domain.Model.Users.Technician || currentUser is Domain.Model.Users.Manager)
                               ? ((TeamEmployee)currentUser).Teams.Where(x => x.Status == Statuses.Active).SelectMany(x => x.Customers).Distinct()
                                    .Select(x => x.ToPersist<Affiliate>()).Where(x => x != null).Where(x => x.Status == Statuses.Active).ToList()
                               : this.affiliatesRepository.Where(x => x.Status == Statuses.Active).ToList();


            var model = EstimateModel.Get();
            if (estimate != null)
            {
                model = EstimateModel.Get(estimate);
                if (estimate.Customer.CustomerType == CustomerType.Retail)
                {
                    var matrices = customers.Count > 0
                                       ? customers.First().Matrices.Where(
                                           x => x.Status == Statuses.Active || x.Id == estimate.Matrix.Id).ToList()
                                       : new List<PriceMatrix>();
                    model.Customer.Wholesale = new EstimateWholesaleCustomerModel(customers, matrices);
                    model.Customer.Retail.Affiliates = ListsHelper.GetAffiliates(affilates, estimate.Affiliate);
                }
                else
                {
                    var customer = customers.SingleOrDefault(x => x.Id == estimate.Customer.Id);
                    if (customer == null)
                    {
                        customer = this.wholesaleCustomersRepository.Get(estimate.Customer.Id);
                        customers.Add(customer);
                    }

                    var matrices = customers.Count > 0
                                       ? estimate.Customer.ToPersist<WholesaleCustomer>().Matrices.Where(
                                           x => x.Status == Statuses.Active || x.Id == estimate.Matrix.Id).ToList()
                                       : new List<PriceMatrix>();
                    model.Customer.Wholesale = new EstimateWholesaleCustomerModel(customers, matrices);
                    model.Customer.Retail.Affiliates = ListsHelper.GetAffiliates(affilates, estimate.Affiliate);
                }
            }
            else
            {
                if (estimateModel != null)
                {
                    model = EstimateModel.Get(model: estimateModel);
                }

                var matrices = customers.Count > 0 ? customers.First().Matrices.Where(x => x.Status == Statuses.Active).ToList() : new List<PriceMatrix>();
                model.Customer.Wholesale = new EstimateWholesaleCustomerModel(customers, matrices);
                model.Customer.Retail.Affiliates = ListsHelper.GetAffiliates(affilates);
            }

            if(!string.IsNullOrEmpty(type))
            {
                model.Customer.Wholesale = new EstimateWholesaleCustomerModel(customers, new List<PriceMatrix>());
                model.Customer.Retail.Affiliates = ListsHelper.GetAffiliates(affilates);
            }
            return model;
        }

    }
}