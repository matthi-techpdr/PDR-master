// -----------------------------------------------------------------------
// <copyright file="CustomButton.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NixJqGridFramework.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using NixJqGridFramework.Entities.Enums;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomButton"/> class.
        /// </summary>
        public CustomButton()
        {
            this.Id = string.Empty;
            this.Caption = string.Empty;
            this.Title = string.Empty;
            this.ButtonIcon = string.Empty;
            this.Cursor = "pointer";

        }

        //optional
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "caption")]
        public string Caption { get; set; }

        [JsonProperty(PropertyName = "buttonicon")]
        public string ButtonIcon { get; set; }

        [JsonProperty(PropertyName = "onClickButton")]
        public string OnClickFunction { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "position")]
        public ButtonPositions? Position { get; set; }

        [JsonProperty(PropertyName = "cursor")]
        public string Cursor { get; set; }
    }
}
