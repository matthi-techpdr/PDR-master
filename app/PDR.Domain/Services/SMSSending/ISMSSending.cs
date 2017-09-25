namespace PDR.Domain.Services.SMSSending
{
    public interface ISMSSending
    {
        void Send(string to, string message);
    }
}
