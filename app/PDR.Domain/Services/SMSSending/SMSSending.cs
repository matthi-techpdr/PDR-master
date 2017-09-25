using System.Configuration;
using Twilio;

namespace PDR.Domain.Services.SMSSending
{
    public class SMSSending : ISMSSending
    {
        private readonly string AuthToken;

        private readonly string AccountSid;

        private readonly string Sender;

        public SMSSending()
        {
            this.AuthToken = ConfigurationManager.AppSettings["AuthToken"];
            this.AccountSid = ConfigurationManager.AppSettings["AccountSID"];
            this.Sender = ConfigurationManager.AppSettings["Sender"];
        }

        public TwilioRestClient Client
        {
            get
            {
                return new TwilioRestClient(this.AccountSid, this.AuthToken);
            }
        }

        public void Send(string to, string message)
        {
            var smsMessage = this.Client.SendSmsMessage(this.Sender, to, message);
        }
    }
}
