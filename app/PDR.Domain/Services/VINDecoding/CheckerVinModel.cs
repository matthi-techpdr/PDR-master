using System;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using SmartArch.Data.Proxy;

namespace PDR.Domain.Services.VINDecoding
{
    using PDR.Domain.Model.Enums;

    public class CheckerVinModel
    {
        protected readonly ICompanyRepository<Estimate> estimatesRepository;

        protected readonly ICompanyRepository<Invoice> invoicesRepository;

        protected readonly ICompanyRepository<RepairOrder> repairOrdersRepository;

        public CheckerVinModel()
        {
            this.estimatesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.repairOrdersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
            this.invoicesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
        }

        public VinModel GetVinModel(string vincode)
        {
            var model = new VinModel();
            model.VinInfo = new VINDecode().Decode(vincode);
            model.IsExistDocument = false;

            vincode = vincode.ToUpper();

            var invoices = invoicesRepository.FirstOrDefault(x => x.RepairOrder.Estimate.Car.VIN.ToUpper() == vincode && !x.IsDiscard);
            if (invoices != null)
            {
                var customerName = String.Empty;
                var wholesaleCustomer = invoices.Customer.ToPersist<WholesaleCustomer>();
                if (wholesaleCustomer != null)
                {
                    customerName = wholesaleCustomer.Name;
                }
                else
                {
                    var retailCustomer = invoices.Customer.ToPersist<RetailCustomer>();
                    if (retailCustomer != null)
                    {
                        customerName = retailCustomer.FirstName + " " + retailCustomer.LastName;
                    }
                }


                model.IsExistDocument = true;
                model.Message = String.Format("Invoice with the same VIN number has been found. Created on {0} for {1} by {2}. Would you like to create new estimate?",
                                    invoices.CreationDate.ToString("M/dd/yyyy"), customerName, invoices.TeamEmployee.Name);
                return model;
            }
            var ro = repairOrdersRepository.FirstOrDefault(x => x.Estimate.Car.VIN.ToUpper() == vincode 
                && x.RepairOrderStatus != RepairOrderStatuses.Finalised && x.RepairOrderStatus != RepairOrderStatuses.Discard);
            if (ro != null)
            {
                var customerName = String.Empty;
                var wholesaleCustomer = ro.Customer.ToPersist<WholesaleCustomer>();
                if (wholesaleCustomer != null)
                {
                    customerName = wholesaleCustomer.Name;
                }
                else
                {
                    var retailCustomer = ro.Customer.ToPersist<RetailCustomer>();
                    if (retailCustomer != null)
                    {
                        customerName = retailCustomer.FirstName + " " + retailCustomer.LastName;
                    }
                }

                model.IsExistDocument = true;
                model.Message = String.Format("RepairOrder with the same VIN number has been found. Created on {0} for {1} by {2}. Would you like to create new estimate?",
                                    ro.CreationDate.ToString("M/dd/yyyy"), customerName, ro.TeamEmployee.Name);
                return model;
            }
            var estimate = estimatesRepository.FirstOrDefault(x => x.Car.VIN.ToUpper() == vincode && x.EstimateStatus != EstimateStatus.Discard 
                && x.EstimateStatus != EstimateStatus.Converted);
            if (estimate != null)
            {
                var customerName = String.Empty;
                var wholesaleCustomer = estimate.Customer.ToPersist<WholesaleCustomer>();
                if (wholesaleCustomer != null)
                {
                    customerName = wholesaleCustomer.Name;
                }
                else
                {
                    var retailCustomer = estimate.Customer.ToPersist<RetailCustomer>();
                    if (retailCustomer != null)
                    {
                        customerName = retailCustomer.FirstName + " " + retailCustomer.LastName;
                    }
                }

                model.IsExistDocument = true;
                model.Message = String.Format("Estimate with the same VIN number has been found. Created on {0} for {1} by {2}. Would you like to create new estimate?",
                                    estimate.CreationDate.ToString("M/dd/yyyy"), customerName, estimate.Employee.Name);
                return model;
            }
            return model;
        }

    }
}
