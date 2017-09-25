using NHibernate.Dialect;

namespace PDR.NHibernateProvider.Dialects
{
    public class CustomMsSqlCe40Dialect : MsSqlCe40Dialect
    {
        public override bool SupportsVariableLimit
        {
            get
            {
                return true;
            }
        }
    }
}