using System;
using System.Linq;

using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Services.Webstorage;

    public class TeamEmployeePercent : CompanyEntity
    {
        public virtual TeamEmployee TeamEmployee { get; set; }

        public virtual double EmployeePart { get; set; }

        public virtual RepairOrder RepairOrder { get; set; }

        public TeamEmployeePercent(bool isNewEntity = false)
        {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public TeamEmployeePercent(){}

        public virtual double GetCommissionSum(Invoice invoice)
        {
            var riSum = this.GetCommissionForRITechnicians();

            var sum = (invoice.GetPaidSumForComission() - riSum) * this.TeamEmployee.Commission * this.EmployeePart * 0.0001;
            return Math.Round(sum, 2);
        }

        private double GetCommissionForRITechnicians()
        {
            var totalLaborSum = RepairOrder.Estimate.CarInspections.Sum(x => x.GetDocumentLaborSum());
            var riPayment = totalLaborSum <= 0
                            ? 0
                            : totalLaborSum * (1 - RepairOrder.Estimate.Discount);

            var riSum = GetRiTechnicianCount() > 0 && RepairOrder.IsFlatFee.HasValue
                ? RepairOrder.IsFlatFee.Value
                    ? RepairOrder.Payment.HasValue
                        ? RepairOrder.Payment.Value : 0
                    : riPayment
                : 0;

            return riSum;
        }

        private int GetRiTechnicianCount()
        {
            return this.RepairOrder.TeamEmployeePercents.Count(x => x.TeamEmployee.Role == UserRoles.RITechnician);
        }

        public virtual double RiPart
        { 
            get
            {
                var countRi = GetRiTechnicianCount();
                return countRi > 0
                    ? Math.Round(this.GetCommissionForRITechnicians() / countRi, 2)
                    : 0;
            }
        }
    }
}