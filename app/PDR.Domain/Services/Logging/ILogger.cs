using NLog;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Services.Logging
{
    public interface ILogger
    {
        void Log(LogLevel level, string msg);

        void Log(Estimate estimate, EstimateLogActions action, string email = null);

        void Log(RepairOrder repairOrder, RepairOrderLogActions action, string email = null);

        void LogRequestEdit(RepairOrder repairOrder);

        void LogEditApproval(RepairOrder repairOrder);

        void LogChangedAdditionalDiscountValue(RepairOrder repairOrder, double oldValue, double newValue);

        void LogChangedLaborRateValue(RepairOrder repairOrder, double oldValue, double newValue);

        void Log(Invoice invoice, InvoiceLogActions action, string email = null);

        void LogPaid(Invoice invoice);

        void LogReassign(Estimate estimate, Employee oldEmployee, Employee doer);

        void LogAssignToRo(RepairOrder repairOrder);

        void LogDefinePercents(RepairOrder repairOrder);

        void LogConvert(Estimate estimate, Employee newEmployee, Employee doer);
    }
}