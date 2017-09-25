using PDR.Domain.Model.Users;

namespace PDR.Domain.Services.PushNotification
{
    public interface IPushNotification
    {
        void Send(Employee employee, string message, string customKey = null, object[] customObjects = null, bool isNewBuildNotificaions = false);

        object GetNewActivities(string teamSelector, Employee currentEmployee);
    }
}
