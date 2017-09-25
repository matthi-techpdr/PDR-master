using PDR.Domain.Model.Base;

namespace PDR.Domain.Model.CustomLines
{
	/// <summary>
	/// Custom line
	/// </summary>
    public class CustomLine : CompanyEntity
	{
		public virtual string Name { get; set; }

		public virtual double Cost { get; set; }
	}
}
