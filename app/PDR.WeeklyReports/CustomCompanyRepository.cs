using System.Linq;
using NHibernate;
using PDR.Domain.Model.Base;
using System.Configuration;

namespace PDR.WeeklyReports
{
    using NHibernate.Linq;

    public class CustomCompanyRepository<T>  where T : ICompanyEntity
    {
        private readonly long companyId;

        private readonly ISession session;

        public CustomCompanyRepository(ISession session)
        {
            var company = ConfigurationManager.AppSettings["companyId"];
            long.TryParse(company, out this.companyId);
            this.session = session;
        }

        public IQueryable<T> All
        {
            get
            {
                return this.session.Query<T>().Where(e => e.Company.Id == companyId);
            }
        }
    }
}
