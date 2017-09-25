namespace PDR.Domain.Services.PushNotification
{
    public interface IPushSettings
    {
        string Certificate { get; set; }

        string Password { get; set; }

        bool Production { get; set; }
    }
}
