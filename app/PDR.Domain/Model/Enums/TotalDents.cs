using PDR.Domain.Model.Attributes;

namespace PDR.Domain.Model.Enums
{
	public enum TotalDents
	{
        [TotalDentsString("1-5")]
		To5,

        [TotalDentsString("6-15")]
		To15,

        [TotalDentsString("16-30")]
		To30,

        [TotalDentsString("31-50")]
		To50,

        [TotalDentsString("51-75")]
		To75,

        [TotalDentsString("76-100")]
		To100,

        [TotalDentsString("101-150")]
		To150,

        [TotalDentsString("151-200")]
		To200,

        [TotalDentsString("201-250")]
        To250,

        [TotalDentsString("251-300")]
        To300,

        [TotalDentsString("No Damage")]
        NoDamage,

        [TotalDentsString("Conventional")]
        Conventional
	}
}
