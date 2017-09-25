// -----------------------------------------------------------------------
// <copyright file="Buttons.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace NixJqGridFramework.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NixJqGridFramework.Entities.Enums;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Buttons
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Buttons"/> class.
        /// </summary>
        public Buttons()
        {
            this.CustomButtons = new List<CustomButton>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show delete button].
        /// </summary>
        public bool ShowDeleteButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show edit button].
        /// </summary>
        public bool ShowEditButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show refresh button].
        /// </summary>
        public bool ShowRefreshButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show add button].
        /// </summary>
        public bool ShowAddButton { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show search button].
        /// </summary>
        public bool ShowSearchButton { get; set; }

        /// <summary>
        /// Gets or sets the custom buttons.
        /// </summary>
        public List<CustomButton> CustomButtons { get; set; }

        /// <summary>
        /// Adds the custom edit button for multiply select.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        public void AddCustomEditButtonForMultiplySelect(string title, ButtonPositions position)
        {
            var customButton = new CustomButton
                {
                    ButtonIcon = "ui-icon-pencil",
                    OnClickFunction = "CustomEditButtonForMultiplySelectFuction",
                    Position = position,
                    Title = title
                };
            this.CustomButtons.Add(customButton);
        }

        public void AddEditButtonWithOnPageEditForm(ButtonPositions position,string elementId)
        {
            var customButton = new CustomButton
            {
                ButtonIcon = "ui-icon-pencil", 
                OnClickFunction = string.Format("GenerateEditFormOnPage", elementId), 
                Position = position, 
                Title = "Edit"
            };
            this.CustomButtons.Add(customButton);
        }
    }
}
