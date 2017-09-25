using NLog;
using PDR.Domain.Model.Matrixes;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public class MatrixLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void Suspend(Matrix matrix)
        {
            var msg = string.Format("Suspend matrix - {0}.", matrix.Name);
            Loggger.Info(msg);
        }

        public static void Reactivate(Matrix matrix)
        {
            var msg = string.Format("Re-activate matrix - {0}.", matrix.Name);
            Loggger.Info(msg);
        }

        public static void Create(Matrix matrix)
        {
            var msg = "Create matrix - " + matrix.Name;
            Loggger.Info(msg);
        }

        public static void Edit(Matrix matrix)
        {
            var msg = "Edit matrix - " + matrix.Name;
            Loggger.Info(msg);
        }

        public static void Duplicate(Matrix matrix)
        {
            var msg = string.Format("Dupicate matrix - {0}.", matrix.Name);
            Loggger.Info(msg);
        }
    }
}