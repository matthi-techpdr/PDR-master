using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using System.Web.Mvc;
using NHibernate;
using PDR.Domain.Wrappers;

namespace PDR.Domain.Services.Account
{
    public class AuthorizationProvider : IAuthorizationProvider
    {
        private readonly INativeRepository<Superadmin> superadminsRepository;

        private readonly INativeRepository<Worker> workersRepository;

        private readonly INativeRepository<Employee> employeesRepository;

        private readonly ICurrentWebStorage<User> userWebStorage;

        private readonly HashGenerator hashGenerator;

        public AuthorizationProvider(
            INativeRepository<Employee> employeesRepository,
            INativeRepository<Superadmin> superadminsRepository,
            ICurrentWebStorage<User> userWebStorage,
            INativeRepository<Worker> workersRepository)
        {
            this.hashGenerator = new HashGenerator();
            this.employeesRepository = employeesRepository;
            this.superadminsRepository = superadminsRepository;
            this.userWebStorage = userWebStorage;
            this.workersRepository = workersRepository;
        }

        public AuthorizationResult LogOnSuperadmin(HttpResponseBase response, string login, string password)
        {
            var super = this.superadminsRepository.SingleOrDefault(u => u.Login == login && u.Password == password);
            if (super != null)
            {
                Func<string, string, Superadmin> superadminQuery = (cmp, lgn) => this.superadminsRepository.SingleOrDefault(u => u.Login == lgn);
                return this.LogOn(response, null, login, password, superadminQuery, superadmin => new AuthorizationResult(true));
            }
            
            Func<string, string, Worker> workerQuery = (cmp, lgn) => this.workersRepository.SingleOrDefault(u => u.Login == lgn);

            return this.LogOn(response, null, login, password, workerQuery, worker => new AuthorizationResult(true));
        }

        public AuthorizationResult LogOnEmployee(HttpResponseBase response, string company, string login, string password, int attemptCount)
        {
            Func<string, string, Employee> employeeQuery =
                (cmp, lgn) =>
                this.employeesRepository.Where(x => x.Status != Statuses.Removed).SingleOrDefault(
                    u => u.Login == lgn && u.Company.Url == company);

            Func<Employee, AuthorizationResult> additionalCheck = employee =>
                {
                    var success = employee.Status == Statuses.Active && employee.Company.Status == Statuses.Active;
                    var message = success ? string.Empty : "Account is suspended";
                    return attemptCount == 4 ? this.SuspendEmployee(employee) : new AuthorizationResult { Success = success, Message = message };
                };

            return this.LogOn(response, company, login, password, employeeQuery, additionalCheck);
        }

        public void LogOut(string token)
        {
            FormsAuthentication.SignOut();
            var dic = HttpContext.Current.Application["Authorize"] as Dictionary<Guid, long>;
            
            if (dic != null)
            {
                var keys = new List<Guid>(dic.Keys);
                foreach (var key in keys)
                {
                    if(!string.IsNullOrEmpty(token))
                    {
                        var guid = Guid.Parse(token);
                        if(guid == key)
                        {
                            dic.Remove(key);
                            token = string.Empty;
                            continue;
                        }
                    }
                    var ticks = dic[key];
                    if (DateTime.Now.Ticks - ticks > new TimeSpan(2, 0, 0).Ticks)
                    {
                        dic.Remove(key);
                    }
                }
            }
        }

        private AuthorizationResult SuspendEmployee(Employee employee)
        {
            if (employee is Wholesaler)
            {
                return new AuthorizationResult {Success = true, Message = string.Empty};
            }

            ISession currentSession = DependencyResolver.Current.GetService<ISession>();
            using (var transaction = currentSession.BeginTransaction())
            {
                employee.Status = Statuses.Suspended;
                transaction.Commit();
                this.employeesRepository.Save(employee);
            }
            return new AuthorizationResult { Success = false, Message = "You have entered an incorrect data 5 times. You have been suspended." };
        }

        private AuthorizationResult LogOn<T>(HttpResponseBase response, string company, string login, string password, Func<string, string, T> userQuery, Func<T, AuthorizationResult> additionalCheck) where T : User
        {
            var user = userQuery.Invoke(company, login);
            if (user != null)
            {
                if (user is Employee)
                {
                    ManagerTabWrapper.IsCompanyTabAvaliable = (user as Employee).CanEditTeamMembers.GetValueOrDefault();
                }
                if (user is Wholesaler)
                {
                    WholesalerTabWrapper.IsAddEstimateTabAvaliable = (user as Wholesaler).CanCreateEstimates;
                }

                AuthorizationResult result = additionalCheck.Invoke(user);
                result.User = user;
                var correctPassword = user.Password == password;

                if (result.Success && correctPassword)
                {
                    this.userWebStorage.Set(user, response);
                    return result;
                }

                if (!result.Success && !(user is Superadmin))
                {
                    return result;
                }

                if(!result.Success && !(user is Worker))
                {
                    return result;
                }
            }

            return new AuthorizationResult { Success = false, Message = "Incorrect login or password" };
        }
    }
}