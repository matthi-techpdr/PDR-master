// -----------------------------------------------------------------------
// <copyright file="FormatOptions.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NixJqGridFramework.Entities.Enums
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Newtonsoft.Json;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FormatOptions
    {
        [JsonProperty(PropertyName = "keys")]
        public bool Keys { get; set; }

        [JsonProperty(PropertyName = "editbutton")]
        public bool EditButton { get; set; }

        [JsonProperty(PropertyName = "delbutton")]
        public bool DeleteButton { get; set; }

        [JsonProperty(PropertyName = "editformbutton")]
        public bool EditFormButton { get; set; }
    }
}
