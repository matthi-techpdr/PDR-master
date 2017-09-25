using System;
using NLog;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public static class ErrorLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        public static void LogError(String error)
        {
            try
            {
                var msg = String.Format(error + Environment.NewLine);
                Loggger.Error(msg);
            }
            catch (Exception ex)
            {
            }
        }
    }
}