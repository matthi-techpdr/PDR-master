namespace PDR.Domain.Services.Account
{
    using System.Web;

    public interface IAuthorizationProvider
    {
        AuthorizationResult LogOnSuperadmin(HttpResponseBase response, string login, string password);

        AuthorizationResult LogOnEmployee(HttpResponseBase response, string company, string login, string password, int attemptCount);

        void LogOut(string token);
    }
}