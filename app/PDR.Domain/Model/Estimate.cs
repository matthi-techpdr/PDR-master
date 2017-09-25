using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Iesi.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.CustomLines;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.ImageManagement;

using SmartArch.Core.Helpers;

namespace PDR.Domain.Model
{
    using PDR.Domain.Services.Webstorage;

    public partial class Estimate : CompanyEntity, IReportable
    {
        public virtual bool New { get; set; }

        public virtual DateTime CreationDate { get; set; }
        
		public virtual Employee Employee { get; set; }

        public virtual Customer Customer { get; set; }

        public virtual Affiliate Affiliate { get; set; }

        public virtual Insurance Insurance { get; set; }

		public virtual Car Car { get; set; }

		public virtual Matrix Matrix { get; set; }

		public virtual Iesi.Collections.Generic.ISet<EstimatePrice> EstimatePrices { get; set; }

		public virtual Iesi.Collections.Generic.ISet<CarInspection> CarInspections { get; set; }

		public virtual Iesi.Collections.Generic.ISet<EstimateCustomLine> CustomEstimateLines { get; set; }

		public virtual Iesi.Collections.Generic.ISet<CarPhoto> Photos { get; set; }

		public virtual string PriorDamages { get; set; }

		public virtual EstimateStatus EstimateStatus { get; set; }

		public virtual bool Signature { get; set; }

		public virtual bool Archived { get; set; }

        public virtual string VINStatus { get; set; }

        public virtual double EstHourlyRate { get; set; }

        public virtual CustomerSignature CustomerSignature { get; set; }

        public virtual double EstDiscount { get; set; }

        public virtual double EstLaborTax { get; set; }

        public virtual double EstMaxCorProtect { get; set; }

        public virtual double EstAluminiumPer { get; set; }

        public virtual double EstOversizedRoofPer { get; set; }

        public virtual double EstDoubleMetalPer { get; set; }

        public virtual double EstLimitForBodyPart { get; set; }

        public virtual double EstOversizedDents { get; set; }

        public virtual double EstMaxPercent { get; set; }

        public virtual double EstCorProtectPart { get; set; }

        public virtual double? ExtraQuickCost { get; set; }

        public virtual EstimateType Type { get; set; }

        public virtual Iesi.Collections.Generic.ISet<Employee> PreviousEmployees { get; set; }

        public virtual CarImage CarImage { get; set; }

        public virtual bool WorkByThemselve { get; set; }

        public virtual double? NewLaborRate { get; set; }

        public virtual string RStatus
        {
            get
            {
                return this.EstimateStatus.ToString();
            }
        }

        public Estimate()
        {
            
        }
        public Estimate(bool isNewEntity = false, Employee employee = null)
        {
            this.CreationDate = SystemTime.Now();
            this.EstimatePrices = new HashedSet<EstimatePrice>();
            this.CarInspections = new HashedSet<CarInspection>();
            this.CustomEstimateLines = new HashedSet<EstimateCustomLine>();
            this.Photos = new HashedSet<CarPhoto>();
            this.PreviousEmployees = new HashedSet<Employee>();
            if (isNewEntity)
            {
                var user = employee ?? ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }

        public virtual CarModel GetCarModel()
        {
            var car = this.Car;
            if (car == null)
            {
                return null;
            }

            var carModels = ServiceLocator.Current.GetInstance<ICompanyRepository<CarModel>>();

            var carModel = carModels
                .FirstOrDefault(x => x.Make == this.Car.Make
                    && x.Model == this.Car.Model
                    && x.YearFrom <= this.Car.Year
                    && x.YearTo >= this.Car.Year);

            return carModel ?? carModels.SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());
        }

        public virtual void DrawCar()
        {
            var carModel = this.GetCarModel();
            var defaultCar = ServiceLocator.Current.GetInstance<ICompanyRepository<CarModel>>()
                                                    .SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());
            var parts = Enum.GetValues(typeof(PartOfBody)).Cast<PartOfBody>();
            var images = new Dictionary<PartOfBody, Image>();
            var isTruck = this.Car.Type == VehicleTypes.Truck;
            var partslist = parts.ToArray();
            for (int i = 0; i < partslist.Length; i++)
            {
                PartColor color;
                CarInspection inspection = this.CarInspections.SingleOrDefault(x => x.Name == partslist[i]);
                if (inspection == null)
                {
                    color = PartColor.Grey;
                }
                else
                {
                    var totalPrice = inspection.GrandTotalWithEffortAndCorrosionProtection();
                    var percent = inspection.Company.LimitForBodyPartEstimate * 0.01;
                    var section = carModel.CarParts.SingleOrDefault(x => x.Name == partslist[i]);
                    var newSectionPrice = section != null
                                              ? section.NewSectionPrice
                                              : defaultCar
                                                  .CarParts
                                                  .Single(x => x.Name == partslist[i])
                                                  .NewSectionPrice;
                    if (totalPrice > 0)
                    {
                        color = newSectionPrice > 0
                                    ? totalPrice < (percent * newSectionPrice)
                                        ? PartColor.Green
                                        : PartColor.Red
                                    : PartColor.Green;
                    }
                    else
                    {
                        color = PartColor.Grey;
                    }
                }

                images.Add(partslist[i], partslist[i].GetImage(color, isTruck));
            }

            var imageManager = new ImageManager();
            var photo = imageManager.DrawCar(images, isTruck);
            this.CarImage = new CarImage(photo, "image/png", true) { Estimate = this };
        }
    }
}

