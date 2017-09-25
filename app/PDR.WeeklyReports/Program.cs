using System;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using PDR.Web.Core.NLog.FileLoggers;
using System.Linq;
using PDR.Domain.Model;

namespace PDR.WeeklyReports
{
    class Program
    {
        static void Main(string[] args)
        { 
            try
            {
                DependencyResolverInit.Initialize();
                var currentSession = ServiceLocator.Current.GetInstance<ISession>();
                var reportManage = new CommonReportManage(currentSession);
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        //week
                        case "-w":
                            {
                                reportManage.CreateWeeklyReports();
                            } break;
                        //month
                        case "-m":
                            {
                                reportManage.CreateMonthlyReports();
                            } break;
                        //summingMonths
                        case "-sm":
                            {
                                reportManage.CreateSummingMonthlyReports();
                            } break;
                        //year
                        case "-y":
                            {
                                reportManage.CreateAnnualReports();
                            } break;

                        default :continue;
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
