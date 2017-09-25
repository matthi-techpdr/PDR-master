using System;
using System.IO;

namespace PDR.Domain.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public abstract class BasePartImageAttribute : Attribute
    {
        private readonly int imageID;

        protected string ImagesFolder { get; set; }

        protected BasePartImageAttribute(int imageID)
        {
            this.imageID = imageID;
            this.ImagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\css\\images\\part-car\\");
        }

        protected string GetUrl(int colorId)
        {
            var fileName = string.Format("{0}-{1}.png", this.imageID, colorId);
            return Path.Combine(this.ImagesFolder, fileName);
        }

        public string Grey
        {
            get
            {
                return this.GetUrl(1);
            }
        }

        public virtual string Red
        {
            get
            {
                return this.GetUrl(2);
            }
        }

        public virtual string Green
        {
            get
            {
                return this.GetUrl(3);
            }
        }
    }
}
