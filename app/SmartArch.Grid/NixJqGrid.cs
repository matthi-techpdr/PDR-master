using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NixJqGridFramework
{
    public class NixJqGrid
    {
        /// <summary>
        /// the columns of grid
        /// </summary>
        public List<NixJqGridColumn> Columns;

        public int RowNumber { get; set; }

        /// <summary>
        /// The pager of grid
        /// </summary>
        private string pager;

        /// <summary>
        /// Sets the pager.
        /// </summary>
        /// <value>
        /// The pager of grid.
        /// </value>
        public string Pager
        {
            set { this.pager = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NixJqGrid"/> class.
        /// </summary>
        /// <param name="colums">The colums.</param>
        public NixJqGrid(List<NixJqGridColumn> colums)
        {
            this.Columns = colums;
        }
    }
}
