using System.Collections.Generic;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;

namespace PDR.Domain.Services.PDFConverters
{
    public interface IPdfConverter
    {
        byte[] Convert(ICompanyEntity entity, bool detailed = false);

        byte[] ReportConvert(DataForReports data, Company company);

        byte[] ConvertRepairOrder(RepairOrder repairOrder, bool detailed = false);

        byte[] ConvertInvoice(Invoice invoice, bool detailed = false);

        byte[] ConvertEstimates(List<Estimate> estimates, bool detailed = false);
    }
}