using System.Linq;
using NLog;
using PDR.Domain.Model;
using System;
using PDR.Domain.Model.Users;

namespace PDR.TaskCheckEmployeeActivity
{
    public static class TeamLogger
    {
        private static readonly Logger Loger = LogManager.GetCurrentClassLogger();

        private static string Action(Team team, string action)
        {
            var msg = string.Format(
                "{0} team(Id = {1}) - {2}; {3}; Members:{4}. Status: {5}.",
                action,
                team.Id,
                team.Title,
                team.Comment,
                string.Join(", ", team.Employees.Select(x => x.Name).ToList()),
                team.Status);
            return msg;
        }

        public static void RemovedEmployeeFromTeam(Team team, TeamEmployee employee)
        {
            var tmp = String.Format("Was removed employee {0} (Id = {1}) from", employee.Name, employee.Id);
            var msg = Action(team, tmp);
            Loger.Info(msg);
        }
    }
}