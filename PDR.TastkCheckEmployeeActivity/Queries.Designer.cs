﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PDR.TaskCheckEmployeeActivity {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Queries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Queries() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("PDR.TaskCheckEmployeeActivity.Queries", typeof(Queries).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DELETE FROM TeamEmployees_Teams WHERE TeamEmployeeFk = {0} AND TeamFk = {1};.
        /// </summary>
        internal static string DeleteEmployeeFromTeam {
            get {
                return ResourceManager.GetString("DeleteEmployeeFromTeam", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string Get {
            get {
                return ResourceManager.GetString("Get", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM Teams WHERE Id IN( SELECT TeamFK FROM Customers_Teams WHERE CustomerFk IN (SELECT CustomerFk 	FROM Estimates WHERE CreationDate &gt; &apos;{1}&apos; AND EmployeeFk = {0} UNION SELECT RepairOrders.CustomerFk FROM RepairOrders, TeamEmployeePercents WHERE RepairOrders.CreationDate &gt; &apos;{1}&apos; AND TeamEmployeePercents.RepairOrderFk = RepairOrders.Id AND TeamEmployeePercents.TeamEmployeeFk = {0}) UNION SELECT Customers_Teams.TeamFK FROM Customers_Teams,  TeamEmployees_Teams WHERE Customers_Teams.CustomerFk NOT IN ( [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetActiveTeams {
            get {
                return ResourceManager.GetString("GetActiveTeams", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM Estimates est WHERE CompanyFk = {0} AND CreationDate &gt; &apos;{1}&apos; AND EmployeeFk = {2} AND EXISTS( SELECT * FROM (SELECT TeamFk FROM Customers_Teams WHERE CustomerFk = est.CustomerFk) t1 INNER JOIN (SELECT TeamFk FROM TeamEmployees_Teams WHERE TeamEmployeeFk = est.EmployeeFk) t2 ON t1.TeamFk = t2.TeamFk );.
        /// </summary>
        internal static string GetEstimates {
            get {
                return ResourceManager.GetString("GetEstimates", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM Users WHERE Class = &apos;Technician&apos; AND CompanyFk = {0} AND Status != 2;.
        /// </summary>
        internal static string GetTechnicians {
            get {
                return ResourceManager.GetString("GetTechnicians", resourceCulture);
            }
        }
    }
}
