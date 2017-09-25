namespace PDR.Domain.Services.TempImageStorage
{
    public class ImageInfo
    {
        public string Id { get; set; }

        public string FullSizeUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public string ContentType { get; set; }
    }
}