using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NixJqGridFramework
{
    public class NixJqGridColumnModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name property of the column.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>
        /// The index property.
        /// </value>
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width property.
        /// </value>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the sorttype.
        /// </summary>
        /// <value>
        /// The sorttype property.
        /// </value>
        public string Sorttype { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NixJqGridColumnModel"/> is sortable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if sortable; otherwise, <c>false</c>.
        /// </value>
        public bool Sortable { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NixJqGridColumnModel"/> class.
        /// </summary>
        public NixJqGridColumnModel()
        {
            this.Name = "name";
            this.Index = "name";
            this.Width = 50;
            this.Sortable = true;
        }
    }
}
