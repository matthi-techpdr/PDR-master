using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using NLog;
using PDR.Domain.Model;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public class TeamLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void Suspend(Team team)
        {
            var msg = string.Format("Suspend team - {0}.", team.Title);
            Loggger.Info(msg);
        }

        public static void Reactivate(Team team)
        {
            var msg = string.Format("Re-activate team - {0}.", team.Title);
            Loggger.Info(msg);
        }

        private static string Action(Team team, string action)
        {
            var msg = string.Format(
                "{0} team - {1}; {2}; Members:{3}. Status: {4}.",
                action,
                team.Title,
                team.Comment,
                string.Join(", ", team.Employees.Select(x => x.Name).ToList()),
                team.Status.ToString());
            return msg;
        }

        public static void Create(Team team)
        {
            var msg = Action(team, "Create");
            Loggger.Info(msg);
        }

        public static void Edit(Team team)
        {
            var msg = Action(team, "Edit");
            Loggger.Info(msg);
        }

        //public static void Edit(Team team, TeamViewModel model)
        //{
        //    var msg = new StringBuilder(string.Format("Edit team - {0}.", team.Title));

        //    if (team.Title != model.Title)
        //    {
        //        msg.Append(string.Format("title: {0}; ", model.Title));
        //    }

        //    if (team.Comment != model.Comments)
        //    {
        //        msg.Append(string.Format("comment: {0}; ", model.Comments));
        //    }

        //    var helper = new LogHelper();
        //    var emploeeCollectionChanges =
        //        helper.GetAddedAndRemovedFromCollectionById(team.Employees.Select(x => x.Id).ToList(), model.EmployeesList.ToList());

        //    var newEmployees = emploeeCollectionChanges["new"];
        //    if (newEmployees.Count() != 0)
        //    {
        //        var eRepo = ServiceLocator.Current.GetInstance<ICompanyRepository<Employee>>();
        //        var names = eRepo.Where(x => newEmployees.Contains(x.Id)).Select(x => x.Name).ToList();
        //        msg.Append(string.Format("New employees: {0}; ", string.Join(", ", names)));
        //    }

        //    var removedEmployees = emploeeCollectionChanges["removed"];
        //    if (removedEmployees.Count() != 0)
        //    {
        //        var eRepo = ServiceLocator.Current.GetInstance<ICompanyRepository<Employee>>();
        //        var names = eRepo.Where(x => removedEmployees.Contains(x.Id)).Select(x => x.Name).ToList();
        //        msg.Append(string.Format("Removed employees: {0}; ", string.Join(", ", names)));
        //    }

        //    Loggger.Info(msg);
        //}
    }
}