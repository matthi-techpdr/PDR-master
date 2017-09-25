using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.CustomLines;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Services.Automapper;
using PDR.Domain.Services.TempImageStorage;

using PDR.Resources.Web;

using PDR.Web.Core.Helpers;

using SmartArch.Data.Proxy;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class EstimateModel : IPhotoContainer
    {
        private static readonly IRepositoryFactory RepositoryFactory =
            ServiceLocator.Current.GetInstance<IRepositoryFactory>();

        public EstimateModel()
        {
            this.Customer = new EstimateCustomerModel();
            this.Insurance = new InsuranceModel();
            this.CarInfo = new CarInfoModel();
            this.CarInspectionsModel = new CarInspectionsModel();
            this.UploadPhotos = new List<ImageInfo>();
            this.StoredPhotos = new List<ImageInfo>();
            this.IsExistVin = false;
        }

        public EstimateModel(Estimate estimate) : this()
        {
            if (estimate == null)
            {
                return;
            }

            var ro = RepositoryFactory.CreateForCompany<RepairOrder>().SingleOrDefault(x => x.Estimate.Id == estimate.Id);

            this.IsRoEditable = ro != null && ro.EditedStatus == EditedStatuses.Editable;
            this.RoId = ro != null ? ro.Id : (long?)null;
            this.IsExistVin = false;

            this.Id = estimate.Id;
            this.Signature = estimate.EstimateStatus == EstimateStatus.Approved || estimate.Signature;
            this.Type = estimate.Type;
            this.Customer = new EstimateCustomerModel();
            this.CarInfo = new CarInfoModel();
            this.TotalAmount = estimate.TotalAmount;
            this.WorkByThemselve = estimate.WorkByThemselve;
            this.CarInspectionsModel = new CarInspectionsModel
                                           {
                                               CalculatedSum = estimate.TotalAmount,
                                               Total = estimate.TotalAmount,
                                               ExtraQuickCost = estimate.ExtraQuickCost
                                           };

            this.Status = estimate.EstimateStatus;
            foreach (var name in GetPartNames())
            {
                CarInspectionsModel.CarInspections.Add(new CarInspectionsInfo
                                                           {
                                                               Name = (PartOfBody) int.Parse(name.Value),
                                                               AverageSize = (AverageSize) (-1),
                                                               TotalDents = (TotalDents) (-1)
                                                           });
            }

            foreach (var inspect in CarInspectionsModel.CarInspections)
            {
                foreach (var carInspect in estimate.CarInspections)
                {
                    if (inspect.Name != carInspect.Name)
                    {
                        continue;
                    }

                    var carLines =
                        carInspect.CustomLines.Where(x => x is CustomCarInspectionLine).Cast<CustomCarInspectionLine>().
                            ToList();
                    var overLines =
                        carInspect.CustomLines.Where(x => x is OversizedDentsLine).Cast<OversizedDentsLine>().ToList();

                    inspect.Id = carInspect.Id;
                    inspect.OptionsPersent = carInspect.OptionsPercent;
                    inspect.TotalDents = carInspect.DentsAmount;
                    inspect.Aluminium = carInspect.Aluminium;
                    inspect.HasAlert = carInspect.HasAlert;
                    inspect.CorrosionProtection = carInspect.CorrosionProtection;
                    inspect.DoubleMetal = carInspect.DoubleMetal;
                    inspect.OversizedRoof = carInspect.OversizedRoof;
                    inspect.AverageSize = carInspect.AverageSize;
                    inspect.DentsCost = carInspect.DentsCost;
                    inspect.PriorDamage = string.IsNullOrWhiteSpace(carInspect.PriorDamage)
                                              ? carInspect.PriorDamage
                                              : carInspect.PriorDamage.Replace("<", "&lt;");
                    inspect.CorrosionProtectionCost = carInspect.CorrosionProtectionCost;
                    inspect.QuickCost = carInspect.QuickCost;

                    foreach (var line in carLines)
                    {
                        var carinspect = new CustomCarInspectionLineModel();
                        CustomAutomapper<CustomCarInspectionLine, CustomCarInspectionLineModel>.Map(line, carinspect);
                        carinspect.Name = carinspect.Name.Replace("<", "&lt;");
                        inspect.CustomCarInspectionLines.Add(carinspect);
                    }

                    foreach (var line in overLines)
                    {
                        inspect.OversizedDentsId = line.Id;
                        inspect.OversizedDents = line.Cost;
                        inspect.AmountOversizedDents = Convert.ToInt32(line.Name);
                    }

                    foreach (var item in carInspect.ChosenEffortItems)
                    {
                        var effortItem = new EffortItemModel
                                             {
                                                 Choosed = item.Choosed,
                                                 Operations = item.Operations,
                                                 Hours = item.Hours,
                                                 Name = item.EffortItem.With(x => x.Name),
                                                 EffortType = item.ChosenEffortType
                                             };

                        inspect.EffortItems.Add(effortItem);
                    }
                }
            }

            this.CarInspectionsModel.PriorDamages = string.IsNullOrWhiteSpace(estimate.PriorDamages)
                                                        ? estimate.PriorDamages
                                                        : estimate.PriorDamages.Replace("<", "&lt;");

            foreach (var line in estimate.CustomEstimateLines)
            {
                var estimateline = new EstimateCustomLineModel();
                CustomAutomapper<EstimateCustomLine, EstimateCustomLineModel>.Map(line, estimateline);
                estimateline.Name = estimateline.Name.Replace("<", "&lt;");
                this.CarInspectionsModel.EstimateCustomLines.Add(estimateline);
            }

            CustomAutomapper<Insurance, InsuranceModel>.Map(estimate.Insurance, this.Insurance);
            this.Insurance.CompanyName = estimate.Insurance.CompanyName.Replace("<", "&lt;");

            CustomAutomapper<Car, CarInfoModel>.Map(estimate.Car, this.CarInfo);

            var retailCustomer = estimate.Customer.ToPersist<RetailCustomer>();
            if (retailCustomer != null)
            {
                CustomAutomapper.Map(retailCustomer, this.Customer.Retail);
                this.Customer.CustomerType = EstimateCustomerType.Retail;
            }
            else
            {
                this.Customer.Wholesale.CustomerId = estimate.Customer.Id;
                if (estimate.Matrix != null)
                {
                    this.Customer.Wholesale.MatrixId = estimate.Matrix.Id;
                }

                this.Customer.CustomerType = EstimateCustomerType.Wholesale;
            }

            this.Subtotal = estimate.Subtotal;
        }

        public long Id { get; set; }

        public double Subtotal { get; set; }

        public CarInfoModel CarInfo { get; set; }

        public EstimateCustomerModel Customer { get; set; }

        public InsuranceModel Insurance { get; set; }

        public CarInspectionsModel CarInspectionsModel { get; set; }

        public EstimateType Type { get; set; }

        public bool Signature { get; set; }

        public double TotalAmount { get; set; }

        public List<ImageInfo> UploadPhotos { get; set; }

        public List<ImageInfo> StoredPhotos { get; set; }

        public EstimateStatus Status { get; set; }

        public bool WorkByThemselve { get; set; }

        public bool IsRoEditable { get; set; }

        public long? RoId { get; set; }

        public bool IsExistVin { get; set; }

        public static EstimateModel Get(Estimate estimate = null, EstimateModel model = null)
        {
            var customers = RepositoryFactory.CreateForCompany<WholesaleCustomer>().Where(x => x.Status == Statuses.Active).ToList();
            var matrices = RepositoryFactory.CreateForCompany<PriceMatrix>().Where(x => x.Status == Statuses.Active).ToList();

            var companyName = (estimate == null ? null : estimate.With(x => x.Insurance).With(x => x.CompanyName))
                              ?? (model == null ? null : model.Insurance.CompanyName);

            var stateCarInfo = (estimate == null
                                    ? null
                                    : estimate.With(x => x.Car).With(x => x.State.GetValueOrDefault().ToString()))
                               ?? (model == null ? null : model.CarInfo.State.ToString());

            var currentType = (estimate == null
                                    ? null
                                    : estimate.With(x => x.Car).With(x => x.Type.GetValueOrDefault().ToString()))
                               ?? (model == null ? null : model.CarInfo.Type.ToString());

            var stateRetailCustomer = (estimate == null
                                           ? null
                                           : estimate.With(x => x.Customer).With(
                                               x => x.State.GetValueOrDefault().ToString())
                                             ?? (model == null ? null : model.Customer.Retail.State.ToString()));

            var companyNames = GetCompanyNames(companyName);

            var updatedModel = model ?? new EstimateModel(estimate);
            updatedModel.Insurance.AllCompanyNames = companyNames;
            updatedModel.Insurance.CompanyName = companyName ?? string.Empty;

            updatedModel.CarInfo.States = GetStates(stateCarInfo);
            updatedModel.CarInfo.VehicleTypes = GetVehicleTypes(currentType);

            updatedModel.Customer.Retail.States = GetStates(stateRetailCustomer);
            //
            updatedModel.Customer.Wholesale.InitCustomers(customers);

            updatedModel.Customer.Wholesale.InitMatrices(matrices);

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
            
            if (estimate != null)
            {
                if (estimate.Photos.Count > 0)
                {
                    updatedModel.StoredPhotos.AddRange(estimate.Photos.ToList().ToImages());
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

        private static IList<SelectListItem> GetVehicleTypes(string currentType = null)
        {
            var types =
                Enum.GetNames(typeof(VehicleTypes))
                    .Select(x => new SelectListItem
                    {
                        Text = x.ToString(),
                        Value = x.ToString(),
                        Selected = currentType != null && x == currentType
                    })
                    .ToList();

            //types[0].Selected = true;

            return types;
        }

        private static IList<SelectListItem> GetStates(string currentState = null)
        {
            var states = Enum.GetNames(typeof(StatesOfUSA))
                .Where(x => x != StatesOfUSA.None.ToString())
                .Select(
                s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = currentState != null && s == currentState
                }).ToList();

            states.Insert(0, new SelectListItem { Text = @" ", Value = "-1" });
            return states;
        }

        private static IList<SelectListItem> GetCompanyNames(string currentName = null)
        {
            var companyNames = RepositoryFactory.CreateForCompany<InsuranceCompany>()
                .Select(x => x.Name)
                .Distinct()
                .ToList();
            companyNames.Sort();
            if (companyNames.Count == 0 && currentName == null)
            {
                companyNames.Insert(0, string.Empty);
                companyNames.Insert(1, "    ");
            }
            else if (companyNames.Count != 0 && currentName == null)
            {
                companyNames.Insert(0, string.Empty);
                companyNames.Insert(1, "No insurance");
            }
            else
            {
                companyNames.Insert(0, "No insurance");
            }

            var selectList = companyNames.Select(companyName => new SelectListItem { Text = companyName.Replace("<", "&lt;") }).Distinct().ToList();

            if (!string.IsNullOrWhiteSpace(currentName))
            {
                var option =
                    selectList.SingleOrDefault(
                        x => x.Text == currentName.Replace("<", "&lt;"));
                if (option != null)
                {
                    option.Selected = true;
                }
                else
                {
                    selectList.Insert(0, new SelectListItem { Text = currentName.Replace("<", "&lt;"), Selected = true });
                }
            }

            return selectList;
        }
	}
}