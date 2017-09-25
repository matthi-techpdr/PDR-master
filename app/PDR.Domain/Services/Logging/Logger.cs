using NLog;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums.LogActions;
using PDR.Domain.Model.Logging;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using SmartArch.Web.Attributes;

namespace PDR.Domain.Services.Logging
{
    public class Logger : ILogger
    {
        private static readonly NLog.Logger Loggger = LogManager.GetCurrentClassLogger();

        private readonly ICompanyRepository<Log> logsRepository;

        private readonly Employee currentEmployee;

        public Logger(ICurrentWebStorage<Employee> currentWebStorage, ICompanyRepository<Log> logsRepository)
        {
            this.currentEmployee = currentWebStorage.Get();
            this.logsRepository = logsRepository;
        }

        [Transaction]
        public void Log(LogLevel level, string msg)
        {
            Loggger.Log(level, msg);
        }


        [Transaction]
        public void Log(Estimate estimate, EstimateLogActions action, string email = null)
        {
            var estimateLog = new EstimateLog(this.currentEmployee, estimate, action, email, true);
            if (action != EstimateLogActions.Edit)
            {
                Loggger.Info(estimateLog.FileLogMessage);
            }
            this.logsRepository.Save(estimateLog);
        }

        [Transaction]
        public void Log(RepairOrder repairOrder, RepairOrderLogActions action, string email = null)
        {
            var repairOrderLog = new RepairOrderLog(this.currentEmployee, repairOrder, action, email, true);
            if (repairOrderLog.Action != RepairOrderLogActions.Edit)
            {
                Loggger.Info(repairOrderLog.FileLogMessage);
            }
            
            this.logsRepository.Save(repairOrderLog);
        }

        [Transaction]
        public void LogRequestEdit(RepairOrder repairOrder)
        {
            var repairOrderLog = new RequestEditRepairOrderLog(repairOrder, this.currentEmployee, true);
            Loggger.Info(repairOrderLog.FileLogMessage);
            this.logsRepository.Save(repairOrderLog);
        }

        [Transaction]
        public void LogEditApproval(RepairOrder repairOrder)
        {
            var repairOrderLog = new EditApprovalRepairOrderLog(repairOrder, this.currentEmployee, true);
            Loggger.Info(repairOrderLog.FileLogMessage);
            this.logsRepository.Save(repairOrderLog);
        }

        [Transaction]
        public void LogChangedAdditionalDiscountValue(RepairOrder repairOrder, double oldValue, double newValue)
        {
            var repairOrderLog = new AddAdditionalDiscountLog(repairOrder, this.currentEmployee, oldValue, newValue, true);
            Loggger.Info(repairOrderLog.FileLogMessage);
            this.logsRepository.Save(repairOrderLog);
        }

        [Transaction]
        public void LogChangedLaborRateValue(RepairOrder repairOrder, double oldValue, double newValue)
        {
            var repairOrderLog = new ChangeLaborRateLog(repairOrder, this.currentEmployee, oldValue, newValue, true);
            Loggger.Info(repairOrderLog.FileLogMessage);
            this.logsRepository.Save(repairOrderLog);
        }

        [Transaction]
        public void Log(Invoice invoice, InvoiceLogActions action, string email = null)
        {
            var invoiceLog = new InvoiceLog(this.currentEmployee, invoice, action, email, true);
            Loggger.Info(invoiceLog.FileLogMessage);
            this.logsRepository.Save(invoiceLog);
        }

        [Transaction]
        public void LogPaid(Invoice invoice)
        {
            var paidInvoiceLog = new PaidInvoiceLog(invoice, this.currentEmployee, true);
            Loggger.Info(paidInvoiceLog.FileLogMessage);
            this.logsRepository.Save(paidInvoiceLog);
        }

        [Transaction]
        public void LogReassign(Estimate estimate, Employee oldEmployee, Employee doer)
        {
            var estimateLog = new ReassignEstimateLog(oldEmployee, estimate, doer, true);
            Loggger.Info(estimateLog.FileLogMessage);
            this.logsRepository.Save(estimateLog);
        }

        [Transaction]
        public void LogConvert(Estimate estimate, Employee newEmployee, Employee doer)
        {
            var estimateLog = new ConvertEstimateLog(newEmployee, estimate, doer, true);
            Loggger.Info(estimateLog.FileLogMessage);
            this.logsRepository.Save(estimateLog);
        }

        [Transaction]
        public void LogAssignToRo(RepairOrder repairOrder)
        {
            var roLog = new AssignRepairOrderLog(repairOrder, this.currentEmployee as TeamEmployee, true);
            Loggger.Info(roLog.FileLogMessage);
            this.logsRepository.Save(roLog);
        }
        
        [Transaction]
        public void LogDefinePercents(RepairOrder repairOrder)
        {
            var definePercentsLog = new DefinePercentsLog(this.currentEmployee as TeamEmployee, repairOrder, true);
            Loggger.Info(definePercentsLog.FileLogMessage);
            this.logsRepository.Save(definePercentsLog);
        }
    }
}
