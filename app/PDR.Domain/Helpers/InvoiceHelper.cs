using System;
using System.Linq;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Helpers
{
    public static class InvoiceHelper
    {
        private static Type[] adminTypes = { typeof(Admin), typeof(Manager), typeof(Accountant) };

        public static double GetCommission(Invoice invoice, Employee currentEmployee)
        {
            double commission = 0;
            var teamEmployee = currentEmployee as TeamEmployee;
            if (teamEmployee != null)
            {
                if (teamEmployee.Role != UserRoles.RITechnician)
                {
                    var teamPercent =
                        invoice.RepairOrder.TeamEmployeePercents.SingleOrDefault(
                            x => Equals(x.TeamEmployee, teamEmployee));
                    if (teamPercent != null)
                    {
                        commission = teamPercent.GetCommissionSum(invoice);
                    }
                }
                else
                {
                    var countRi = invoice.RepairOrder.TeamEmployeePercents.Count(x => x.TeamEmployee.Role == UserRoles.RITechnician);
                    double sum = 0;
                    if (invoice.RepairOrder.IsFlatFee.HasValue)
                    {
                        var riPayment = invoice.RepairOrder.Estimate.CarInspections.Sum(x => x.GetDocumentLaborSum()) <= 0
                        ? 0 : invoice.RepairOrder.Estimate.CarInspections.Sum(x => x.GetDocumentLaborSum()) * (1 - invoice.RepairOrder.Estimate.DocumentDiscount);

                        sum = invoice.RepairOrder.IsFlatFee.Value
                                  ? (invoice.RepairOrder.Payment.HasValue ? invoice.RepairOrder.Payment.Value : 0)
                                  : riPayment;
                    }
                    commission = (sum / countRi);
                }
            }

            if (adminTypes.Contains(currentEmployee.GetType()))
            {
                commission = invoice.GetCommission();
            }

            return commission >= 0 ? commission : 0;
        }
    }
}
