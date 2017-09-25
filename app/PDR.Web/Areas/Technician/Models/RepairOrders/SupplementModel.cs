using System.Web.Mvc;

namespace PDR.Web.Areas.Technician.Models.RepairOrders
{
    public class SupplementModel
    {
        public long Id { get; set; }
        [AllowHtml]
        public string Description { get; set; }

        public string Sum { get; set; }
    }
}