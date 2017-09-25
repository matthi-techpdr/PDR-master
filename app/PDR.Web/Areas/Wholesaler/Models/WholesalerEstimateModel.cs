using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Services.TempImageStorage;
using PDR.Web.Areas.Estimator.Models.Estimates;

namespace PDR.Web.Areas.Wholesaler.Models
{
    public class WholesalerEstimateModel : IPhotoContainer
    {
        private static readonly IRepositoryFactory RepositoryFactory =
            ServiceLocator.Current.GetInstance<IRepositoryFactory>();

        public WholesalerEstimateModel()
        {
            //this.Customer = new EstimateCustomerModel();
            this.Employee = new EstimateEmployeeModel();
            this.Insurance = new InsuranceModel();
            this.CarInfo = new CarInfoModel();
            this.CarInspectionsModel = new CarInspectionsModel();
            this.UploadPhotos = new List<ImageInfo>();
            this.StoredPhotos = new List<ImageInfo>();
            this.IsExistVin = false;
            this.WholesaleCustomer = new WholesaleCustomerModel();
            this.Matrices = new List<SelectListItem>();
        }

        public WholesalerEstimateModel(PDR.Domain.Model.Customers.WholesaleCustomer wholesaler, IEnumerable<PDR.Domain.Model.Users.Employee> employees)
        {
            this.Employee=new EstimateEmployeeModel(employees);
            this.Insurance = new InsuranceModel();
            this.CarInfo = new CarInfoModel();
            this.CarInspectionsModel = new CarInspectionsModel();
            this.UploadPhotos = new List<ImageInfo>();
            this.StoredPhotos = new List<ImageInfo>();
            this.IsExistVin = false;
            this.WholesaleCustomer = new WholesaleCustomerModel(wholesaler);
            //this.Matrices = new List<SelectListItem>();
            InitMatrices(wholesaler.Matrices.Where(x => x.Status == Statuses.Active));
        }

        public long Id { get; set; }

        public double Subtotal { get; set; }

        public CarInfoModel CarInfo { get; set; }

        public EstimateEmployeeModel Employee { get; set; }

        public IList<SelectListItem> Matrices { get; set; }

        public long MatrixId { get; set; }

        public InsuranceModel Insurance { get; set; }

        public CarInspectionsModel CarInspectionsModel { get; set; }

        public EstimateType Type { get; set; }

        public bool Signature { get; set; }

        public double TotalAmount { get; set; }

        public List<ImageInfo> UploadPhotos { get; set; }

        public List<ImageInfo> StoredPhotos { get; set; }

        public EstimateStatus Status { get; set; }

        public bool WorkByThemselve { get; set; }

        public bool IsExistVin { get; set; }

        public WholesaleCustomerModel WholesaleCustomer { get; set; }

        public static WholesalerEstimateModel Get(PDR.Domain.Model.Customers.WholesaleCustomer wholesaler, IEnumerable<PDR.Domain.Model.Users.Employee> employees)
        {
            var companyNames = GetCompanyNames();

            var updatedModel = new WholesalerEstimateModel(wholesaler, employees);
            updatedModel.Insurance.AllCompanyNames = companyNames;

            updatedModel.CarInfo.States = GetStates();
            updatedModel.CarInfo.VehicleTypes = GetVehicleTypes();

            updatedModel.CarInspectionsModel.FullPartsNames = GetFullPartsName();
            updatedModel.CarInspectionsModel.PartsNames = GetPartNames();
            updatedModel.CarInspectionsModel.AverageSize = GetAverageSize(null);
            updatedModel.CarInspectionsModel.TotalDents = GetTotalDents(null);

            if (updatedModel.CarInspectionsModel.CarInspections.Count == 0)
            {
                foreach (var name in GetPartNames())
                {
                    updatedModel.CarInspectionsModel.CarInspections.Add(new CarInspectionsInfo
                                                                            {
                                                                                Name = (PartOfBody)int.Parse(name.Value),
                                                                                AverageSize = (AverageSize)(-1),
                                                                                TotalDents = (TotalDents)(-1)
                                                                            });
                }
            }
            return updatedModel;
        }

        private static IList<SelectListItem> GetPartNames()
        {
            var partNames =
                Enum.GetValues(typeof(PartOfBody)).Cast<int>()
                    .Select(x => new SelectListItem { Text = ((PartOfBody)x).ToString(), Value = x.ToString() })
                    .ToList();

            partNames[0].Selected = true;

            return partNames;
        }

        private static IList<string> GetFullPartsName()
        {
            return new List<string>
                                   {
                                       "Hood",
                                       "Roof",
                                       "Deck lid",
                                       "Right fender",
                                       "Left fender",
                                       "Right front door",
                                       "Right rear door",
                                       "Right cab corner",
                                       "Left front door",
                                       "Left rear door",
                                       "Left cab corner",
                                       "Right quarter",
                                       "Left quarter",
                                       "Metal sunroof",
                                       "Left roof rail",
                                       "Right roof rail",                           
                                       "Cowl/Other",
                                       "Front bumper",
                                       "Rear bumper",
                                   };
        }

        private static IList<string> GetTotalDentsForView()
        {
            return new List<string> { "1-5", "6-15", "16-30", "31-50", "51-75", "76-100", "101-150", "151-200", "201-250", "251-300", "No Damage", "Conventional" };
        }

        private static IList<SelectListItem> GetAverageSize(int? currentSize)
        {
            var averageSize = Enum.GetValues(typeof(AverageSize)).Cast<int>().Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = ((AverageSize)s).ToString(),
                    Selected = currentSize != null && s == currentSize
                }).ToList();

            averageSize.Insert(0, new SelectListItem { Selected = true, Text = @"Choose...", Value = "-1" });
            return averageSize;
        }

        private static IList<SelectListItem> GetTotalDents(int? currentDents)
        {
            var totalDents = Enum.GetValues(typeof(TotalDents)).Cast<int>().Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = ((TotalDents)s).ToString(),
                Selected = currentDents != null && s == currentDents
            }).ToList();

            for (var i = 0; i < totalDents.Count; i++)
            {
                totalDents[i].Text = GetTotalDentsForView()[i];
            }

            totalDents.Insert(0, new SelectListItem { Text = @"Choose...", Value = "-1" });
            return totalDents;
        }

        private static IList<SelectListItem> GetVehicleTypes()
        {
            var types =
                Enum.GetNames(typeof(VehicleTypes))
                    .Select(x => new SelectListItem
                    {
                        Text = x.ToString(),
                        Value = x.ToString(),
                    })
                    .ToList();

            return types;
        }

        private static IList<SelectListItem> GetStates()
        {
            var states = Enum.GetNames(typeof(StatesOfUSA))
                .Where(x => x != StatesOfUSA.None.ToString())
                .Select(
                s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                }).ToList();

            states.Insert(0, new SelectListItem { Text = @" ", Value = "-1" });
            return states;
        }

        private static IList<SelectListItem> GetCompanyNames()
        {
            var companyNames = RepositoryFactory.CreateForCompany<InsuranceCompany>()
                .Select(x => x.Name)
                .Distinct()
                .ToList();
            companyNames.Sort();
            if (companyNames.Count == 0)
            {
                companyNames.Insert(0, string.Empty);
                companyNames.Insert(1, "    ");
            }
            else if (companyNames.Count != 0)
            {
                companyNames.Insert(0, string.Empty);
                companyNames.Insert(1, "No insurance");
            }
            else
            {
                companyNames.Insert(0, "No insurance");
            }

            var selectList = companyNames.Select(companyName => new SelectListItem { Text = companyName.Replace("<", "&lt;") }).Distinct().ToList();

            return selectList;
        }

        public void InitMatrices(IEnumerable<Matrix> matrices)
        {
            this.Matrices = matrices.OrderBy(x => x.Name).Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();
        }
	}
}