using System.Web.Mvc;

namespace PDR.Web.Areas.SuperAdmin.Models
{
    public class SendPushNotificationViewModel
    {
       [AllowHtml]
       public string Message { get; set; }
    }
}