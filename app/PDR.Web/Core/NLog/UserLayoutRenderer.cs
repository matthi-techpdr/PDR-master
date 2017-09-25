using System.Text;
using Microsoft.Practices.ServiceLocation;
using NLog;
using NLog.LayoutRenderers;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;

namespace PDR.Web.Core.LogConfig
{
    [LayoutRenderer("user")]
    public class UserLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get() ?? ServiceLocator.Current.GetInstance<ICurrentWebStorage<User>>().Get();
            if (user != null)
            {
                builder.Append(user.Login);
            }
        }
    }
}