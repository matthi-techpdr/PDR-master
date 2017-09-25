
namespace NixJqGridFramework.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using NixJqGridFramework.Entities;
    using NixJqGridFramework.Entities.Enums;

    /// <summary>
    /// NixJqGrid column model.
    /// </summary>
    public class NixJqGridColumnModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NixJqGridColumnModel"/> class.
        /// </summary>
        public NixJqGridColumnModel()
        {
            this.Width = 70;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NixJqGridColumnModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="index">The index.</param>
        public NixJqGridColumnModel(string name, string index)
            : this()
        {
            this.Name = name;
            this.Index = index;
            this.EditType = null;
            this.Formatter = null;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        [JsonProperty(PropertyName = "index")]
        public string Index { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NixJqGridColumnModel"/> is editable.
        /// </summary>
        [JsonProperty(PropertyName = "editable")]
        public bool Editable { get; set; }

        /// <summary>
        /// Gets or sets the type of the edit.
        /// </summary>
        [JsonProperty(PropertyName = "edittype")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EditTypes? EditType { get; set; }

        /// <summary>
        /// Gets or sets the edit options.
        /// </summary>
        public string EditOptionsUrl { get; set; }

        /// <summary>
        /// Gets or sets the edit rules.
        /// </summary>
        [JsonProperty(PropertyName = "editrules")]
        public EditRules EditRules { get; set; }

        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        [JsonProperty(PropertyName = "formatter")]
        [JsonConverter(typeof(StringEnumConverter))]
        public Formatters? Formatter { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom formatter function.
        /// </summary>
        public string CustomFormatterFunctionName { get; set; }

        /// <summary>
        /// Gets or sets the name of the custom unformatter function.
        /// </summary>
        public string CustomUnformatterFunctionName { get; set; }

        /// <summary>
        /// Gets or sets the date format.
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NixJqGridColumnModel"/> is hidden.
        /// </summary>
        [JsonProperty(PropertyName = "hidden")]
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets FormatOptions.
        /// </summary>
        [JsonProperty(PropertyName = "formatoptions")]
        public FormatOptions FormatOptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NixJqGridColumnModel"/> is searchable.
        /// </summary>
        [JsonProperty(PropertyName = "search")]
        public bool Searchable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Sortable.
        /// </summary>
        [JsonProperty(PropertyName = "sortable")]
        public bool Sortable { get; set; }
    }
}
