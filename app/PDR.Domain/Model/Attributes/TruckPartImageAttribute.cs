using System.IO;

namespace PDR.Domain.Model.Attributes
{
    public class TruckPartImageAttribute : BasePartImageAttribute
    {
        public TruckPartImageAttribute(int imageID)
            : base(imageID)
        {
            this.ImagesFolder = Path.Combine(this.ImagesFolder, "Truck");
        }

        public override string Red
        {
            get
            {
                return this.GetUrl(3);
            }
        }

        public override string Green
        {
            get
            {
                return this.GetUrl(2);
            }
        }
    }
}