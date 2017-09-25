using PDR.Domain.Model.Attributes;

namespace PDR.Domain.Model.Enums
{
	public enum AverageSize
	{
        [AverageSizeString("Dime")]
		DIME,
        [AverageSizeString("Nickel")]
		NKL,
        [AverageSizeString("Quarter")]
		QTR,
        [AverageSizeString("Half")]
		HALF
	}
}
