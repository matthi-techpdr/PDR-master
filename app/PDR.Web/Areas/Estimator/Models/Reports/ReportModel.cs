namespace PDR.Web.Areas.Estimator.Models.Reports
{
    public class ReportModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public long? TeamId { get; set; }

        public long? CustomerId { get; set; }

        public bool Commission { get; set; }

        public string Role { get; set; }

        public string CustomerName { get; set; }

        public string TeamName { get; set; }
    }
}