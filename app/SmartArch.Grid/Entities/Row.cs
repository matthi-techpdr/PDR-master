// -----------------------------------------------------------------------
// <copyright file="Row.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NixJqGridFramework.Entities
{
    using System.Collections.Generic;
    using NixJqGridFramework.NixJqGridCore;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Row
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        public Row()
        {
            this.RowList = new List<int>() { 5, 10, 15 };
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NixJqGrid"/> is rownumbers.
        /// </summary>
        public bool Rownumbers { get; set; }

        /// <summary>
        /// Gets or sets the rowlist.
        /// </summary>
        public List<int> RowList { get; set; }

        /// <summary>
        /// Gets or sets the row numbers.
        /// </summary>
        public int RowNumber { get; set; }
    }
}
