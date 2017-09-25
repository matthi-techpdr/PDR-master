using System;
using System.Configuration;
using System.Reflection;

namespace SmartArch.Core.Helpers
{
    public static class AssemblyVersionHelper
    {
        private static string cachedCurrentVersion;

        private static string IsLiveKey = "IsLive";

        private static bool? cachedIsLive;

        public static Type TypeFromAssembly { get; set; }

        /// <summary>
        /// Return the Current Version from the AssemblyInfo.cs file.
        /// </summary>
        public static string CurrentVersion
        {
            get
            {
                if (cachedCurrentVersion == null)
                {
                    try
                    {
                        var assembly = TypeFromAssembly != null
                                           ? Assembly.GetAssembly(TypeFromAssembly)
                                           : Assembly.GetExecutingAssembly();

                        cachedCurrentVersion = assembly.GetName().Version.ToString();
                    }
                    catch
                    {
                        cachedCurrentVersion = "?.?.?.?";
                    }
                }

                return cachedCurrentVersion;
            }
        }

        public static bool IsLive
        {
            get
            {
                if (cachedIsLive == null)
                {
                   cachedIsLive = Convert.ToBoolean(ConfigurationManager.AppSettings[IsLiveKey]);
                }
                return cachedIsLive.Value;
            }
        }
    }
}