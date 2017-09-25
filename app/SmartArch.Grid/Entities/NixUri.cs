using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NixJqGridFramework.NixJqGridSettings
{
    /// <summary>
    /// class for url which can be counfigured in jqgrid
    /// </summary>
    public class NixUri
    {
        /// <summary>
        /// Gets or sets the get data action.
        /// </summary>
        /// <value>
        /// The get data action.
        /// </value>
        public string GetDataAction { get; set; }

        /// <summary>
        /// Gets or sets the edit data action.
        /// </summary>
        /// <value>
        /// The edit data action.
        /// </value>
        public string EditDataAction { get; set; }

        /// <summary>
        /// Gets or sets the delete data action.
        /// </summary>
        /// <value>
        /// The delete data action.
        /// </value>
        public string DeleteDataAction { get; set; }

        /// <summary>
        /// Gets or sets the add data action.
        /// </summary>
        /// <value>
        /// The add data action.
        /// </value>
        public string AddDataAction { get; set; }
    }
}
