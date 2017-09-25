using PDR.Domain.Model.Enums.Attributes;

namespace PDR.Domain.Model.Enums
{
    public enum InvoiceStatus
    {
        [Description("Paid")]
        Paid,
        [Description("Unpaid")]
        Unpaid,
        [Description("Paid In Full")]
        PaidInFull
    }
}
