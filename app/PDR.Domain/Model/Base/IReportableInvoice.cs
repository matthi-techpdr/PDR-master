namespace PDR.Domain.Model.Base
{
    public interface IReportableInvoice : IReportable
    {
        double PaidSum { get; set; }

        double GetCommission();
    }
}
