using System.Collections.Generic;
using System.IO;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using PDR.Domain.Model;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Reports;

namespace PDR.Domain.Services.PDFConverters
{
    public class PdfConverter : IPdfConverter
    {
        private static byte[] DocumentToByteArray(Document document)
        {
            if (document != null)
            {
                var memory = new MemoryStream();
                var pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always) { Document = document };
                pdfRenderer.RenderDocument();
                pdfRenderer.PdfDocument.Save(memory, true);
                return memory.ToArray();
            }

            return null;
        }

        public byte[] Convert(ICompanyEntity entity, bool detailed = false)
        {
            Document document = null;
            if (entity is Estimate)
            {
                var estimateConverter = new EstimateToPdfConverter(entity as Estimate);
                document = estimateConverter.CreateDocument(detailed);
            }
            else if (entity is EstimateReport)
            {
            }

            return DocumentToByteArray(document);
        }

        public byte[] ConvertRepairOrder(RepairOrder repairOrder, bool detailed = false)
        {
            var estimateConverter = new EstimateToPdfConverter(repairOrder.Estimate, repairOrder);
            var document = estimateConverter.CreateDocument(detailed);
            return DocumentToByteArray(document);
        }

        public byte[] ConvertInvoice(Invoice invoice, bool detailed = false)
        {
            var estimateConverter = new EstimateToPdfConverter(invoice.RepairOrder.Estimate, invoice.RepairOrder, invoice);
            var document = estimateConverter.CreateDocument(detailed);
            return DocumentToByteArray(document);
        }

        public byte[] ReportConvert(DataForReports data, Company company)
        {
            var reportConverter = new ReportToPdfConverter(data);
            var document = reportConverter.CreateDocument(company);

            return DocumentToByteArray(document);
        }

        public byte[] ConvertEstimates(List<Estimate> estimates, bool detailed = false)
        {
            var estimateConverter = new EstimateToPdfConverter(estimates);
            var document = estimateConverter.CreateDocument(detailed);

            return DocumentToByteArray(document);
        }
    }
}
