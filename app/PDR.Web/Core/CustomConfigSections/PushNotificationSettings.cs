using System;
using System.Configuration;
using PDR.Domain.Services.PushNotification;

namespace PDR.Web.Core.CustomConfigSections
{
    public class PushNotificationSettings : ConfigurationSection, IPushSettings
    {
        //Current certificare HTPDRPushProd.p12 is valid till the 9th of January than it should be replaced by the new one
        [ConfigurationProperty("certificate")]
        public string Certificate
        {
            get { return (string)this["certificate"]; }
            set { this["certificate"] = value; }
        }

        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("production")]
        public bool Production
        {
            get { return Convert.ToBoolean(this["production"]); }
            set { this["production"] = value; }
        }
    }
}