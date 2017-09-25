using System;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;

namespace PDR.Domain.Model.Reports
{
    public abstract class Report : CompanyEntity
    {
        public virtual string Title { get; set; }

        public virtual string StartDate { get; set; }

        public virtual string EndDate { get; set; }

        public virtual long? TeamId { get; set; }

        public virtual long? CustomerId { get; set; }

        public virtual bool Commission { get; set; }

        public virtual Employee Employee { get; set; }

        public virtual ReportType ReportType
        {
            get
            {
                return (ReportType)Enum.Parse(typeof(ReportType), this.GetType().Name);
            }
        }
    }
}
