using Iesi.Collections.Generic;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Matrixes
{
	public abstract class Matrix : CompanyEntity
	{
		public virtual string Name { get; set; }

		public virtual string Description { get; set; }

		public virtual int AluminiumPanel { get; set; }

		public virtual int DoubleLayeredPanels { get; set; }

		public virtual int OversizedRoof { get; set; }

		public virtual int Maximum { get; set; }

		public virtual double CorrosionProtectionPart { get; set; }

		public virtual double MaxCorrosionProtection { get; set; }

        public virtual ISet<MatrixPrice> MatrixPrices { get; set; }

        public virtual double OversizedDents { get; set; }

	    public virtual Statuses Status { get; set; }

        public Matrix()
        {
            this.MatrixPrices = new HashedSet<MatrixPrice>();
        }

        public virtual void AddMatrixPrice(MatrixPrice matrixPrice)
        {
            this.MatrixPrices.Add(matrixPrice);
            matrixPrice.Matrix = this;
        }
	}
}
