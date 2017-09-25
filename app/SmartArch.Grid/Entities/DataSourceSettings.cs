namespace NixJqGridFramework.Entities
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using NixJqGridFramework.Entities.Enums;
    using NixJqGridFramework.Models;

    /// <summary>
    /// Data source settings.
    /// </summary>
    public class DataSourceSettings
    {
        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        public IList<NixJqGridColumn> Columns { get; set; }

        /// <summary>
        /// Gets or sets the datatype.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DataTypes DataType { get; set; }

        /// <summary>
        /// Gets or sets the type of the method.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MethodTypes MethodType { get; set; }

        /// <summary>
        /// Gets ColumnNames.
        /// </summary>
        public IEnumerable<string> ColumnNames
        {
            get
            {
                return this.Columns.Select(n => n.Name);
            }
        }

        /// <summary>
        /// Gets ColumnModels.
        /// </summary>
        public IEnumerable<NixJqGridColumnModel> ColumnModels
        {
            get
            {
                return this.Columns.Select(n => n.NixJqGridColumnModel);
            }
        }

        /// <summary>
        /// Adds the action column.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="deleteButton">
        /// The delete Button.
        /// </param>
        /// <param name="editButton">
        /// The edit Button.
        /// </param>
        public void AddEditDeleteActionColumn(string name, int width, bool deleteButton, bool editButton)
        {
            var columns = new NixJqGridColumn(name);
            var actionColumn = new NixJqGridColumnModel
                {
                    Name = name,
                    Width = width,
                    Formatter = Formatters.actions,
                    Index = (this.Columns.Count() + 1).ToString(),
                    FormatOptions =
                        new FormatOptions
                            { DeleteButton = deleteButton, EditButton = editButton, EditFormButton = true, Keys = true },
                    Searchable = false,
                    Sortable = false,
                };
            columns.NixJqGridColumnModel = actionColumn;
            this.Columns.Add(columns);
        }

        /// <summary>
        /// Add custom column.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="formatterFuncName">
        /// The formatter Func Name.
        /// </param>
        /// <param name="unformatterFuncName">
        /// The unformatter Func Name.
        /// </param>
        public void AddCustomColumn(string name, int width, string formatterFuncName, string unformatterFuncName)
        {
            var columns = new NixJqGridColumn(name);
            var actionColumn = new NixJqGridColumnModel
                {
                    Name = name,
                    Width = width,
                    Formatter = Formatters.customFormatter,
                    Index = (this.Columns.Count() + 1).ToString(),
                    Searchable = false,
                    Sortable = false,
                    CustomFormatterFunctionName = formatterFuncName,
                    CustomUnformatterFunctionName = unformatterFuncName
                };
            columns.NixJqGridColumnModel = actionColumn;
            this.Columns.Add(columns);
        }
    }
}