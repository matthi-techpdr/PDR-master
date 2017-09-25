using System;
using System.Configuration;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using PDR.Domain.Model;
using PDR.Domain.Model.Users;
using PDR.Web.Core.NLog.FileLoggers;

namespace PDR.TaskCheckEmployeeActivity
{

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                DependencyResolverInit.Initialize();
                var currentSession = ServiceLocator.Current.GetInstance<ISession>();
                int companyId;
                bool result = Int32.TryParse(ConfigurationManager.AppSettings["companyId"], out companyId);

                var query = String.Format(Queries.GetTechnicians, companyId);
                var technicians = currentSession.CreateSQLQuery(query)
                    .AddEntity(typeof(TeamEmployee))
                    .List<TeamEmployee>();

                for (var i = 0; i < technicians.Count; i++)
                {
                    var lowDate = DateTime.Now.AddDays(-20);
                    var teams = technicians[i].Teams.ToArray();
                    if (teams.Length == 0) continue;
                    query = String.Format(Queries.GetActiveTeams, technicians[i].Id, lowDate);
                    var activeTeams = currentSession.CreateSQLQuery(query)
                        .AddEntity(typeof(Team))
                        .List<Team>();
                    for (var j = 0; j < teams.Length; j++)
                    {
                        var teamName = teams[j].Title.ToLower();
                        if (teamName.Contains("test") || teamName.Contains("training") || teamName.ToLower() == "all") continue;

                        if (!activeTeams.Contains(teams[j]))
                        {
                            query = String.Format(Queries.DeleteEmployeeFromTeam, technicians[i].Id, teams[j].Id);
                            var res = currentSession.CreateSQLQuery(query).ExecuteUpdate();
                            if (res > 0)
                            {
                                TeamLogger.RemovedEmployeeFromTeam(teams[j], technicians[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorLogger.LogError(exception.ToString());
            }
        }
    }
}

