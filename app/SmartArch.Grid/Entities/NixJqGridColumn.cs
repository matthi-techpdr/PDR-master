
namespace NixJqGridFramework.Entities
{
    using NixJqGridFramework.Models;

    /// <summary>
    /// Nix jqGrid column.
    /// </summary>
    public class NixJqGridColumn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NixJqGridColumn"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public NixJqGridColumn(string name = "Column")
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the nix jq grid column model.
        /// </summary>
        public NixJqGridColumnModel NixJqGridColumnModel { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        public string ColumnType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [from to filter].
        /// </summary>
        public bool FromToFilter { get; set; }
    }
}
