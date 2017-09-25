
namespace NixJqGridFramework.NixJqGridCore
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using NixJqGridFramework.Entities;
    using NixJqGridFramework.Entities.Enums;
    using NixJqGridFramework.NixJqGridSettings;

    /// <summary>
    /// The NixJqGrid
    /// </summary>
    public class NixJqGrid
    {
        /// <summary>
        /// Gets or sets the actions URL.
        /// </summary>
        public NixUri ActionsUrl { get; set; }

        /// <summary>
        /// Gets or sets the data source settings.
        /// </summary>
        public DataSourceSettings DataSourceSettings { get; set; }

        /// <summary>
        /// Gets or sets the row info.
        /// </summary>
        public Row RowInfo { get; set; }

        /// <summary>
        /// Gets or sets Buttons.
        /// </summary>
        public Buttons Buttons { get; set; }

        /// <summary>
        /// Gets or sets the multiple.
        /// </summary>
        /// <value>
        /// The multiple.
        /// </value>
        public Multiple Multiple { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [multi select].
        /// </summary>
        public bool MultiSelect { get; set; }

        /// <summary>
        /// Gets or sets the datatype.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public DataTypes DataType { get; set; }

        /// <summary>
        /// Gets or sets the sortorder.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SortOrders Sortorder { get; set; }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets the pager.
        /// </summary>
        public string PagerId { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto width].
        /// </summary>
        [JsonProperty(PropertyName = "autowidth")]
        public bool AutoWidth { get; set; }

        /// <summary>
        /// Gets or sets the table id.
        /// </summary>
        public string TableId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NixJqGrid"/> is loadonce.
        /// </summary>
        public bool Loadonce { get; set; }

        /// <summary>
        /// Gets or sets the sortname.
        /// </summary>
        public string Sortname { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NixJqGrid"/> is viewrecords.
        /// </summary>
        public bool Viewrecords { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has filter toolbar.
        /// </summary>
        public bool HasFilterToolbar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [in line editable].
        /// </summary>
        public bool InLineEditable { get; set; }

        /// <summary>
        /// Gets or sets the name of the in line editing function.
        /// </summary>
        public string InLineEditingFunctionName
        {
            get
            {
                if (this.InLineEditable)
                {
                    return "onSelectRowFunction";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the name of the post data function.
        /// </summary>
        public string PostDataFunctionName
        {
            get
            {
                if (HasFilterToolbar)
                {
                    return "PostDataFunction";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the has filter toolbar function.
        /// </summary>
        public string HasFilterToolbarFunction
        {
            get
            {
                if (HasFilterToolbar)
                {
                    return "filterToolbar";
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the on grid complete function.
        /// </summary>
        public string onGridCompleteFunction
        {
            get
            {
                bool flag = false;
                foreach (var column in this.DataSourceSettings.Columns)
                {
                    if (column.FromToFilter)
                    {
                        flag = true;
                        
                    }
                }
                if (flag)
                {
                    return "onGridCompleteFunction";
                }
                else
                {
                    return string.Empty;
                }
                
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [footer row].
        /// </summary>
        public bool FooterRow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [view sortable columns].
        /// </summary>
        public bool ViewSortableColumns { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NixJqGrid"/> class.
        /// </summary>
        /// <param name="caption">
        /// The caption.
        /// </param>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="dss">
        /// The dss.
        /// </param>
        public NixJqGrid(string caption, NixUri url,  DataSourceSettings dss)
        {
            this.Caption = caption;
            this.DataSourceSettings = dss;
            this.ActionsUrl = url;
            this.TableId = "NixGrid_" + Guid.NewGuid().ToString();
            this.PagerId = "NixPager_" + Guid.NewGuid().ToString();
            this.DataType = DataTypes.json;
            this.Sortorder = SortOrders.asc;
            this.RowInfo = new Row { RowNumber = 10 };
            this.Multiple = new Multiple();
            this.AutoWidth = true;
        }
    }
}
