using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Matrixes
{
	public abstract class Price : CompanyEntity
    {
        public virtual PartOfBody PartOfBody { get; set; }

        public virtual AverageSize AverageSize { get; set; }

        public virtual TotalDents TotalDents { get; set; }

        public virtual double Cost { get; set; }
    } 
}
