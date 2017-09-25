using MigraDoc.DocumentObjectModel;

using PDR.Domain.Model;

namespace PDR.Domain.Services.PDFConverters
{
    public class RepairOrderToPdfConverter 
    {
        private readonly RepairOrder repairOrder;

        public RepairOrderToPdfConverter(RepairOrder ro)
        {
            this.repairOrder = ro;
        }

        public Document CreateDocument(string logoUrl, string carUrl, string signatuteUrl)
        {
            var estimateConverter = new EstimateToPdfConverter(this.repairOrder.Estimate, this.repairOrder);
            var document = estimateConverter.CreateDocument();
            return document;
        }
    }
}
