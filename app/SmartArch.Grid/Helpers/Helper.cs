// -----------------------------------------------------------------------
// <copyright file="Helper.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NixJqGridFramework.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Bools to string.
        /// </summary>
        /// <param name="boolObj">if set to <c>true</c> [bool obj].</param>
        /// <returns>
        /// String with lower first letter.
        /// </returns>
        public static string BoolToString(this bool boolObj)
        {
            return boolObj.ToString().ToLower();
        }
    }
}
