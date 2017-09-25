using System.Collections.Generic;
using System.Net.Mail;

namespace PDR.Domain.Services.Mailing
{
    public interface IMailService
    {
        string Send(string from, string to, string subject, string message, IList<Attachment> attachments = null);
    }
}
