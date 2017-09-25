using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Logging;

namespace PDR.Domain.Helpers
{
    public class DocumentHistoryManager
    {
        private readonly ICompanyRepository<Estimate> estimateRepository;

        private readonly ICompanyRepository<RepairOrder> repairOrderRepository;

        private readonly ICompanyRepository<Invoice> invoiceRepository;

        private readonly ICompanyRepository<EntityLog<Estimate>> entityLogForEstimatesRepository;

        private readonly ICompanyRepository<EntityLog<RepairOrder>> entityLogForRepairOrdersRepository;

        private readonly ICompanyRepository<EntityLog<Invoice>> entityLogForInvoicesRepository;

        public DocumentHistoryManager()
        {
            estimateRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            repairOrderRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<RepairOrder>>();
            invoiceRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Invoice>>();
            entityLogForEstimatesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<EntityLog<Estimate>>>();
            entityLogForRepairOrdersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<EntityLog<RepairOrder>>>();
            entityLogForInvoicesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<EntityLog<Invoice>>>();
        }

        public IEnumerable<string> GenerateHistory<TModel>(TModel document, bool withEmployeeName) where TModel : CompanyEntity    
        {
            var historyLog = new List<string>();

            var repairOrder = document as RepairOrder;
            if (repairOrder != null)
            {
                historyLog = this.GetLogMessageByRepairOrder(repairOrder, withEmployeeName).ToList();
                return historyLog;
            }
            
            var invoice = document as Invoice;
            if (invoice != null)
            {
                historyLog = this.GetLogMessageByInvoice(invoice, withEmployeeName).ToList();
                return historyLog;
            }

            var estimate = document as Estimate;
            if (estimate != null)
            {
                historyLog = GetLogMessageByEstimate(estimate, withEmployeeName).ToList();
                return historyLog;
            }
            return historyLog;
        }

        private IEnumerable<string> GetLogMessageByEstimate(Estimate estimate, bool withEmployeeName)
        {
            var result = new Dictionary<KeyValuePair<string, long>, string>();
            if (estimate != null)
            {
                var estLogs = this.GetEntityLogForEstimate(estimate, withEmployeeName);
                estLogs.ForEach(x => result.Add(x.Key, x.Value));
            }

            var repairOrder = repairOrderRepository.FirstOrDefault(x => x.Estimate.Id == estimate.Id);
            if (repairOrder != null)
            {
                var roLogs = this.GetEntityLogForRepairOrder(repairOrder, withEmployeeName);
                roLogs.ForEach(x => result.Add(x.Key, x.Value));
            }
            
            var invoice = invoiceRepository.FirstOrDefault(x => x.RepairOrder.Estimate.Id == estimate.Id);
            if (invoice != null)
            {
                var invoiceLogs = this.GetEntityLogForInvoice(invoice, withEmployeeName);
                invoiceLogs.ForEach(x => result.Add(x.Key, x.Value));
            }
            var logs = result.OrderBy(key => key.Key.ToString()).Select(x => x.Value);
            return logs;
        }

        private IEnumerable<string> GetLogMessageByRepairOrder(RepairOrder repairOrder, bool withEmployeeName)
        {
            var result = new Dictionary<KeyValuePair<string, long>, string>();
            if (repairOrder != null)
            {
                var roLogs = this.GetEntityLogForRepairOrder(repairOrder, withEmployeeName);
                roLogs.ForEach(x => result.Add(x.Key, x.Value));
            }

            var estimate = estimateRepository.FirstOrDefault(x => x.Id == repairOrder.Estimate.Id);
            if (estimate != null)
            {
                var estLogs = this.GetEntityLogForEstimate(estimate, withEmployeeName);
                estLogs.ForEach(x => result.Add(x.Key, x.Value));
            }

            var invoice = invoiceRepository.FirstOrDefault(x => x.RepairOrder.Id == repairOrder.Id);
            if (invoice != null)
            {
                var invoiceLogs = this.GetEntityLogForInvoice(invoice, withEmployeeName);
                invoiceLogs.ForEach(x => result.Add(x.Key, x.Value));
            }
            var logs = result.OrderBy(key => key.Key.ToString()).Select(x => x.Value);
            return logs;
        }

        private IEnumerable<string> GetLogMessageByInvoice(Invoice invoice, bool withEmployeeName)
        {
            var result = new Dictionary<KeyValuePair<string, long>, string>();

            if (invoice != null)
            {
                var invoiceLogs = this.GetEntityLogForInvoice(invoice, withEmployeeName);
                invoiceLogs.ForEach(x => result.Add(x.Key, x.Value));
            }

            var estimate = estimateRepository.FirstOrDefault(x => x.Id == invoice.RepairOrder.Estimate.Id);
            if (estimate != null)
            {
                var estLogs = this.GetEntityLogForEstimate(estimate, withEmployeeName);
                estLogs.ForEach(x => result.Add(x.Key, x.Value));
            }

            var repairOrder = repairOrderRepository.FirstOrDefault(x => x.Estimate.Id == estimate.Id);
            if (repairOrder != null)
            {
                var roLogs = this.GetEntityLogForRepairOrder(repairOrder, withEmployeeName);
                roLogs.ForEach(x => result.Add(x.Key, x.Value));
            }

            var logs = result.OrderBy(key => key.Key.ToString()).Select(x => x.Value);
            return logs;
        }

        private Dictionary<KeyValuePair<string, long>, string> GetEntityLogForEstimate(Estimate estimate, bool withEmployeeName)
        {
            var result = new Dictionary<KeyValuePair<string, long>, string>();
            var logs = entityLogForEstimatesRepository.Where(x => x.EntityId == estimate.Id).ToArray();
            for (int i = 0; i < logs.Length; i++)
            {
                result.Add(new KeyValuePair<string, long>(logs[i].Date.ToString("yyyy-MM-dd"), logs[i].Id), logs[i].GetLogMessage(withEmployeeName));
            }
            return result;
        }

        private Dictionary<KeyValuePair<string, long>, string> GetEntityLogForRepairOrder(RepairOrder repairOrder, bool withEmployeeName)
        {
            var result = new Dictionary<KeyValuePair<string, long>, string>();
            var logs = entityLogForRepairOrdersRepository.Where(x => x.EntityId == repairOrder.Id).ToArray();
            for (int i = 0; i < logs.Length; i++)
            {
                result.Add(new KeyValuePair<string, long>(logs[i].Date.ToString("yyyy-MM-dd"), logs[i].Id), logs[i].GetLogMessage(withEmployeeName));
            }
            return result;
        }

        private Dictionary<KeyValuePair<string, long>, string> GetEntityLogForInvoice(Invoice invoice, bool withEmployeeName)
        {
            var result = new Dictionary<KeyValuePair<string, long>, string>();
            var logs = entityLogForInvoicesRepository.Where(x => x.EntityId == invoice.Id).ToArray();
            for (int i = 0; i < logs.Length; i++)
            {
                result.Add(new KeyValuePair<string, long>(logs[i].Date.ToString("yyyy-MM-dd"), logs[i].Id), logs[i].GetLogMessage(withEmployeeName));
            }
            return result;
        }
    }
}
