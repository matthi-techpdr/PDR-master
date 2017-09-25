using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Services.ImageManagement
{
    public class ImageManager
    {
        private static readonly Dictionary<PartOfBody, Coordinates> CarPartsCoordinates = new Dictionary<PartOfBody, Coordinates>()
            {
                { PartOfBody.LFender, new Coordinates(0, 10) },
                { PartOfBody.LFDoor, new Coordinates(0, 114) },
                { PartOfBody.LRDoor, new Coordinates(0, 227) },
                { PartOfBody.LQuarter, new Coordinates(0, 301) },
                { PartOfBody.FrontBumper, new Coordinates(78, 0) }, 
                { PartOfBody.Hood, new Coordinates(95, 20) },
                { PartOfBody.Roof, new Coordinates(122, 160) },
                { PartOfBody.LtRoofRail, new Coordinates(95, 151) },
                { PartOfBody.RtRoofRail, new Coordinates(232, 152) },
                { PartOfBody.DeckLid, new Coordinates(115, 325) },
                { PartOfBody.RearBumper, new Coordinates(93, 335) },
                { PartOfBody.RFender, new Coordinates(278, 10) },
                { PartOfBody.RFDoor, new Coordinates(253, 114) },
                { PartOfBody.RRDoor, new Coordinates(253, 227) },
                { PartOfBody.RQuarter, new Coordinates(265, 297) }
            };

        private static readonly Dictionary<PartOfBody, Coordinates> TruckPartsCoordinates = new Dictionary<PartOfBody, Coordinates>()
            {
                { PartOfBody.LFender, new Coordinates(0, 10) },
                { PartOfBody.LFDoor, new Coordinates(0, 109) },
                { PartOfBody.LRDoor, new Coordinates(0, 212) },
                { PartOfBody.LCabCorner, new Coordinates(0, 280) },
                { PartOfBody.LQuarter, new Coordinates(0, 305) },

                { PartOfBody.FrontBumper, new Coordinates(95, 0)}, 
                { PartOfBody.Hood, new Coordinates(105, 20) },
                { PartOfBody.Roof, new Coordinates(122, 150) },
                { PartOfBody.LtRoofRail, new Coordinates(105, 141) },
                { PartOfBody.RtRoofRail, new Coordinates(225, 142) },
                { PartOfBody.DeckLid, new Coordinates(116, 315) },
                { PartOfBody.RearBumper, new Coordinates(103, 375) },

                { PartOfBody.RFender, new Coordinates(275, 3) },
                { PartOfBody.RFDoor, new Coordinates(250, 111) },
                { PartOfBody.RRDoor, new Coordinates(250, 214) },
                { PartOfBody.RCabCorner, new Coordinates(250, 282) },
                { PartOfBody.RQuarter, new Coordinates(280, 307) }
                
            };

        private readonly Bitmap carImage;

        private readonly Graphics graphics;

        public ImageManager()
        {
            this.carImage = new Bitmap(360, 455);
            this.graphics = Graphics.FromImage(this.carImage);
        }

        private void DrawPart(Image img, int x, int y)
        {
            if (img != null)
            {
                this.graphics.DrawImage(img, x, y, img.Width, img.Height);
            }
        }

        private byte[] ImageToBytes(Bitmap image)
        {
            var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);
            var bytes = memoryStream.ToArray();
            return bytes;
        }

        public byte[] DrawCar(Dictionary<PartOfBody, Image> images, bool isTruck)
        {
            var partsCoordinates = isTruck ? TruckPartsCoordinates : CarPartsCoordinates;

            using (this.graphics)
            {
                foreach (var partCoordinate in partsCoordinates)
                {
                    PartOfBody part = partCoordinate.Key;
                    var coordinate = partCoordinate.Value;
                    this.DrawPart(images[part], coordinate.X, coordinate.Y);
                }
            }

            return this.ImageToBytes(this.carImage);
        }
    }
}
