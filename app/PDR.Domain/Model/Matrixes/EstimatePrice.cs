namespace PDR.Domain.Model.Matrixes
{
	public class EstimatePrice : Price
	{
		public virtual Estimate Estimate { get; set; }
	}
}
