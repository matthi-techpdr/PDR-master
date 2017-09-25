using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace PDR.Domain.Services.Mailing
{
    public class MailService : IMailService
    {
        private readonly SmtpClient smtp; 
        
        public MailService()
        {
            this.smtp = new SmtpClient();
        }

        public string Send(string from, string to, string subject, string message, IList<Attachment> attachments = null)
        {
            var adresses = to.Split(',').Select(x => x.Trim());

            var mes = string.Empty;
            
            foreach (var adress in adresses)
            {
                var mailMessage = new MailMessage { Subject = subject, Body = message };
                if (!string.IsNullOrWhiteSpace(from))
                {
                    mailMessage.From = new MailAddress(from);
                }

                if (attachments != null)
                {
                    foreach (var attachment in attachments)
                    {
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                mailMessage.To.Add(adress);
                try
                {
                    this.smtp.Send(mailMessage);
                }
                catch (SmtpException ex)
                {
                    mes = string.Empty; //// "Recipient verify failed!";
                }
            }

            return mes;
        }
    }
}
