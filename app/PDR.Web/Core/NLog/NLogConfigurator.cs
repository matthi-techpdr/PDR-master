using NLog.Config;

using PDR.Web.Core.LogConfig;

namespace PDR.Web.Core.NLog
{
    public static class NLogConfigurator
    {
        public static void Init()
        {
            InitCustomLayoutRenderers();
        }

        private static void InitCustomLayoutRenderers()
        {
            ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition("user", typeof(UserLayoutRenderer));
        }
    }
}