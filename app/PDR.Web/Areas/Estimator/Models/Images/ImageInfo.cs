namespace PDR.Web.Areas.Estimator.Models.Images
{
    public class ImageInfo
    {
        /// <summary>
        /// Gets or sets the virtual path to full size image.
        /// </summary>
        /// <value>
        /// The virtual path to full size image.
        /// </value>
        public string FullSize { get; set; }

        /// <summary>
        /// Gets or sets the virtual path to thumbnail of image.
        /// </summary>
        /// <value>
        /// The virtual path to thumbnail of image.
        /// </value>
        public string Thumbnail { get; set; }
    }
}