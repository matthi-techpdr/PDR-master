using System.IO;

using PDR.Domain.Model.Base;

namespace PDR.Domain.Model.Photos
{
    public abstract class Photo : CompanyEntity
    {
        protected Photo()
	    {
	        this.Name = string.Empty;
	    }

        protected Photo(string fullsizePath, string thumbailPath, string contentType)
                    : this()
        {
            this.ContentType = contentType;
            this.PhotoFull = File.ReadAllBytes(fullsizePath);
            if (thumbailPath != null)
            {
                this.PhotoThumbnail = File.ReadAllBytes(thumbailPath); 
            }
        }

        protected Photo(byte[] photo, string contentType) : this()
        {
            this.PhotoFull = photo;
            this.PhotoThumbnail = photo;
            this.ContentType = contentType;
        }

        public virtual string ContentType { get; set; }

		public virtual byte[] PhotoFull { get; set; }

		public virtual byte[] PhotoThumbnail { get; set; }

		public virtual string Name { get; set; }
    }
}