using System;
using System.Collections.Generic;
using System.Linq;

using Iesi.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.CustomLines;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Automapper;

using PDR.Web.WebAPI.IphoneModels;

using SmartArch.Data;
using SmartArch.Data.Proxy;

namespace PDR.Web.WebAPI.Helper
{
    using PDR.Domain.Services.Webstorage;

    public class EstimateModelToEstimateConverter
    {
        private readonly ICompanyRepository<Employee> employeesRepository;

        private readonly ICompanyRepository<Customer> customersRepository;

        private readonly ICompanyRepository<EffortItem> effortItemsRepository;

        private readonly ICompanyRepository<Matrix> matrixesRepository;

        private readonly ICompanyRepository<Estimate> Repository;

        private readonly ICompanyRepository<EstimateCustomLine> estimateCustomLines;

        private readonly ICompanyRepository<CustomCarInspectionLine> carInspectionLines;

        private readonly ICompanyRepository<OversizedDentsLine> oversizedDentsLines;

        private readonly ICompanyRepository<CustomLine> customLines; 

        private readonly ICompanyRepository<ChosenEffortItem> chosenEffortItems;

        private readonly ICompanyRepository<EffortItem> effortItems;

        private readonly ICompanyRepository<Affiliate> affiliates;

        private readonly ICompanyRepository<Car> cars;

        private readonly ICompanyRepository<CarInspection> carInspections;

        private readonly ICompanyRepository<Insurance> insurances;

        private readonly ICompanyRepository<CarImage> images;

        private readonly ICompanyRepository<InsuranceCompany> insuranceCompanies;

        private readonly ICurrentWebStorage<Employee> storage;


        public EstimateModelToEstimateConverter()
        {
            this.matrixesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Matrix>>();
            this.customersRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Customer>>();
            this.employeesRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<Employee>>();
            this.effortItemsRepository = ServiceLocator.Current.GetInstance<ICompanyRepository<EffortItem>>();
            this.Repository = ServiceLocator.Current.GetInstance<ICompanyRepository<Estimate>>();
            this.customLines = ServiceLocator.Current.GetInstance<ICompanyRepository<CustomLine>>();
            this.estimateCustomLines = ServiceLocator.Current.GetInstance<ICompanyRepository<EstimateCustomLine>>();
            this.carInspectionLines = ServiceLocator.Current.GetInstance<ICompanyRepository<CustomCarInspectionLine>>();
            this.oversizedDentsLines = ServiceLocator.Current.GetInstance<ICompanyRepository<OversizedDentsLine>>();
            this.chosenEffortItems = ServiceLocator.Current.GetInstance<ICompanyRepository<ChosenEffortItem>>();
            this.effortItems = ServiceLocator.Current.GetInstance<ICompanyRepository<EffortItem>>();
            this.carInspections = ServiceLocator.Current.GetInstance<ICompanyRepository<CarInspection>>();
            this.cars = ServiceLocator.Current.GetInstance<ICompanyRepository<Car>>();
            this.insurances = ServiceLocator.Current.GetInstance<ICompanyRepository<Insurance>>();
            this.images = ServiceLocator.Current.GetInstance<ICompanyRepository<CarImage>>();
            this.affiliates = ServiceLocator.Current.GetInstance<ICompanyRepository<Affiliate>>();
            this.storage = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>();
            this.insuranceCompanies = ServiceLocator.Current.GetInstance<ICompanyRepository<InsuranceCompany>>();
        }

        public Estimate GetEstimate(ApiEstimateModel model)
        {
            var estimate = model.Id != null ? this.Repository.Get(Convert.ToInt64(model.Id)) : new Estimate(true);
            
            long? oldMatrixId = null;
            long? oldCustomerId = null;
            if(model.Id != null)
            {
                oldMatrixId = estimate.Matrix.Id;
                oldCustomerId = estimate.Customer.Id;
            }
            this.AddCar(model, estimate);

            this.AddCustomer(model, estimate);
            this.AddInsurance(model, estimate);
            this.AddMatrix(model, estimate);
            if (model.RetailCustomer != null)
            {
                this.AddAffiliate(model, estimate);
            }
            this.AddEmployee(model, estimate);
            this.AddOthers(model, estimate);

            this.AddCustomLines(model, estimate);
            this.AddCarInspections(model, estimate, oldCustomerId, oldMatrixId);

            //// remove carImage for PDF
            if (estimate.CarImage != null)
            {
                var id = estimate.CarImage.Id;
                estimate.CarImage = null;
                var img = images.Get(id);
                images.Remove(img);
            }

            if (estimate.Matrix != null)
            {
                estimate.SaveCalculation();
            }
            else
            {
                estimate.TotalAmount = estimate.CalculateTotalAmount();
            }

            return estimate;
        }

        #region Helpers

        private void AddCar(ApiEstimateModel model, Estimate estimate)
        {
            var car = estimate.Car ?? new Car(true);
            car.Color = model.Car.Color;
            car.CustRO = model.Car.CustRO;
            car.VIN = model.Car.VIN;
            car.LicensePlate = model.Car.LicensePlate;
            car.Make = model.Car.Make;
            car.Mileage = model.Car.Mileage;
            car.Model = string.IsNullOrEmpty(model.Car.Model)
                        ? model.Car.Model
                        : model.Car.Model.Replace("&lt;", "<");
            car.Stock = model.Car.Stock;
            car.Year = model.Car.Year;
            car.Trim = model.Car.Trim;
            StatesOfUSA carState;

            if (Enum.TryParse(model.Car.State, true, out carState))
            {
                car.State = carState;
            }

            car.Type = !string.IsNullOrEmpty(model.Car.Type)
                       ? (VehicleTypes)Enum.Parse(typeof(VehicleTypes), model.Car.Type)
                       : VehicleTypes.Car;

            this.cars.Save(car);
            estimate.Car = car;
        }

        private void AddCustomer(ApiEstimateModel model, Estimate estimate)
        {
            if(model.CustomerId != null)
            {
                var customer = this.customersRepository.Get(model.CustomerId.Value).ToPersist<WholesaleCustomer>();
                estimate.Customer = customer;
                estimate.WorkByThemselve = customer.WorkByThemselve;
            }
            else if(model.RetailCustomer != null)
            {
                var retail = model.RetailCustomer.Id.HasValue
                                 ? this.customersRepository.Get(model.RetailCustomer.Id.Value).ToPersist<RetailCustomer>()
                                 : new RetailCustomer(true);

                retail.Address1 = model.RetailCustomer.Address1;
                retail.Address2 = model.RetailCustomer.Address2;
                retail.Email = model.RetailCustomer.Email;
                retail.Fax = model.RetailCustomer.Fax;
                retail.FirstName = model.RetailCustomer.FirstName;
                retail.LastName = model.RetailCustomer.LastName;
                retail.Phone = model.RetailCustomer.Phone;
                retail.City = model.RetailCustomer.City;
                retail.Zip = model.RetailCustomer.Zip;
                retail.State = model.RetailCustomer.State != null
                                   ? (StatesOfUSA) Enum.Parse(typeof (StatesOfUSA), model.RetailCustomer.State)
                                   : StatesOfUSA.None;

                estimate.Customer = retail;
                estimate.WorkByThemselve = false;
            }
        }

        private void AddInsurance(ApiEstimateModel model, Estimate estimate)
        {
            var insurance = estimate.Insurance ?? new Insurance(true) { CompanyName = "No insurance" };
            if (model.Insurance != null)
            {
                insurance.InsuredName = model.Insurance.InsuredName;
                insurance.AccidentDate = model.Insurance.AccidentDate != null
                                             ? (DateTime?) Convert.ToDateTime(model.Insurance.AccidentDate)
                                             : null;
                insurance.Claim = string.IsNullOrWhiteSpace(model.Insurance.Claim)
                                                    ? model.Insurance.Claim
                                                    : model.Insurance.Claim.Replace("&lt;", "<");
                insurance.ClaimDate = model.Insurance.ClaimDate != null
                                          ? (DateTime?) Convert.ToDateTime(model.Insurance.ClaimDate)
                                          : null;
                insurance.CompanyName = model.Insurance.CompanyName;
                insurance.ContactName = model.Insurance.ContactName;
                insurance.Phone = model.Insurance.Phone;
                insurance.Policy = model.Insurance.Policy;
            }
            this.insurances.Save(insurance);
            estimate.Insurance = insurance;
            insurance.Estimate = estimate;

            var employee = this.storage.Get();
            if (employee is Admin)
            {
                var companyName = insurance.CompanyName;
                var isExist = insuranceCompanies.FirstOrDefault(x => x.Name == companyName);
                var tmp = String.Compare("no insurance", companyName, true) != 0 && !string.IsNullOrWhiteSpace(companyName);
                if (isExist == null && tmp)
                {
                    var item = new InsuranceCompany(true);
                    item.Name = companyName;
                    insuranceCompanies.Save(item);
                }
            }
        }

        private void AddMatrix(ApiEstimateModel model, Estimate estimate)
        {
            Matrix matrix = null;

            if (model.MatrixId == null)
            {
                if (estimate.Customer.CustomerType == CustomerType.Wholesale)
                {
                    matrix = (estimate.Customer.ToPersist<WholesaleCustomer>()).Matrices.FirstOrDefault();
                }
            }
            else
            {
                matrix = this.matrixesRepository.Get(model.MatrixId.Value);
            }

            estimate.Matrix = matrix;
        }

        private void AddEmployee(ApiEstimateModel model, Estimate estimate)
        {
            if (model.EmployeeId == null)
            {
                return;
            }

            var employee = this.employeesRepository.Get(model.EmployeeId.Value);
            estimate.Employee = employee;
        }

        private void AddOthers(ApiEstimateModel model, Estimate estimate)
        {
            estimate.PriorDamages = model.PriorDamages;
            estimate.VINStatus = model.VINStatus;
            estimate.ExtraQuickCost = model.ExtraQuickCost;
            estimate.Type = (EstimateType)Enum.Parse(typeof(EstimateType), model.Type);

            estimate.EstimateStatus = model.Status != null
                                          ? (EstimateStatus)Enum.Parse(typeof(EstimateStatus), model.Status)
                                          : EstimateStatus.Open;

            if (model.Archived != null)
            {
                estimate.Archived = model.Archived.Value;
            }
        }

        private void AddCustomLines(ApiEstimateModel model, Estimate estimate)
        {
            var estimateCustomLinetemp = new HashedSet<EstimateCustomLine>();
            var estimateCustomLineRemove = new List<EstimateCustomLine>();

            if (estimate.Type == EstimateType.ExtraQuick || estimate.Type == EstimateType.Quick)
            {
                return;
            }

            if(model.CustomLines == null)
            {
                return;
            }

            foreach (var t in model.CustomLines)
            {
                //// && t.Cost != null
                if (string.IsNullOrEmpty(t.Name))
                {
                    continue;
                }

                var estimateCustomLine = estimate.CustomEstimateLines.IsEmpty
                                             ? new EstimateCustomLine(true)
                                             : estimate.CustomEstimateLines.SingleOrDefault(x => x.Id == t.Id)
                                               ?? new EstimateCustomLine(true);

                CustomAutomapper<ApiCustomFieldModel, EstimateCustomLine>.Map(t, estimateCustomLine);
                //estimateCustomLine.Name = estimateCustomLine.Name.Replace("&lt;", "<");

                this.estimateCustomLines.Save(estimateCustomLine);
                estimateCustomLinetemp.Add(estimateCustomLine);
                estimateCustomLine.Estimate = estimate;
            }

            foreach (var estimateCustomLine in estimateCustomLinetemp)
            {
                estimate.CustomEstimateLines.Add(estimateCustomLine);
            }

            if (estimateCustomLinetemp.Count != 0)
            {
                estimateCustomLineRemove.AddRange(estimate.CustomEstimateLines.Where(item => !estimateCustomLinetemp.Contains(item)));
            }
            else
            {
                estimateCustomLineRemove = estimate.CustomEstimateLines.ToList();
            }

            foreach (var line in estimateCustomLineRemove)
            {
                estimate.CustomEstimateLines.Remove(line);
                this.estimateCustomLines.Remove(line);
            }
        }

        private void AddQuickCarInspections(ApiEstimateModel model, Estimate estimate, long? oldCustomerId, long? oldMatrixId)
        {
            var inspectTemp = new HashedSet<CarInspection>();
            var inspectTempRemove = new HashedSet<CarInspection>();

            if (model.CarInspections == null || estimate.Matrix.Id != oldMatrixId || estimate.Customer.Id != oldCustomerId)
            {
                inspectTempRemove.AddAll(estimate.CarInspections);
            }

            if (model.CarInspections != null)
            {
                foreach (var inspection in model.CarInspections)
                {
                    if (inspection.QuickCost == 0)
                    {
                        var inspect =
                            this.carInspections.SingleOrDefault(x => x.Id == Convert.ToInt64(inspection.Id));
                        if (inspect != null)
                        {
                            inspectTempRemove.Add(inspect);
                        }

                        continue;
                    }

                    var carInspection = estimate.CarInspections.IsEmpty
                                            ? new CarInspection(true)
                                            : estimate.CarInspections.SingleOrDefault(x => x.Id == inspection.Id)
                                              ?? new CarInspection(true);
                    carInspection.Estimate = estimate;
                    carInspection.QuickCost = inspection.QuickCost;

                    carInspection.Name = inspection.Name;
                    carInspection.Aluminium = false;
                    carInspection.DoubleMetal = false;
                    carInspection.OversizedRoof = false;
                    carInspection.CorrosionProtection = false;
                    carInspection.AverageSize = (AverageSize) (-1);
                    carInspection.DentsAmount = (TotalDents) (-1);
                    carInspection.DentsCost = 0;

                    this.carInspections.Save(carInspection);
                    inspectTemp.Add(carInspection);
                }
            }

            foreach (var carInspection in inspectTemp)
            {
                estimate.CarInspections.Add(carInspection);
            }

            foreach (var carInspection in inspectTempRemove)
            {
                this.carInspections.Remove(carInspection);
                estimate.CarInspections.Remove(carInspection);
            }
        }

        private void AddNormalCarInspections(ApiEstimateModel model, Estimate estimate, long? oldCustomerId, long? oldMatrixId)
        {
            var inspectTemp = new HashedSet<CarInspection>();
            var inspectTempRemove = new HashedSet<CarInspection>();
            
            if (model.CarInspections == null || estimate.Matrix.Id != oldMatrixId || estimate.Customer.Id != oldCustomerId)
            {
                inspectTempRemove.AddAll(estimate.CarInspections);
            }
            
            if(model.CarInspections != null)
            {
                if (estimate.Car.Type.Value == VehicleTypes.Car)
                {
                    inspectTempRemove.AddAll(estimate.CarInspections
                        .Where(x => x.Name == PartOfBody.LCabCorner || x.Name == PartOfBody.RCabCorner).ToList());
                }

                foreach (var carInspectionModel in model.CarInspections)
                {
                    if (carInspectionModel.PartsTotal == 0 && carInspectionModel.DentsAmount != TotalDents.NoDamage &&
                        carInspectionModel.DentsAmount != TotalDents.Conventional)
                    {
                        var inspect =
                            this.carInspections.SingleOrDefault(x => x.Id == Convert.ToInt64(carInspectionModel.Id));
                        if (inspect != null)
                        {
                            inspectTempRemove.Add(inspect);
                        }

                        continue;
                    }

                    var carInspection = estimate.CarInspections.IsEmpty
                                            ? new CarInspection(true)
                                            : estimate.CarInspections.SingleOrDefault(x => x.Id == carInspectionModel.Id)
                                              ?? new CarInspection(true);

                    this.AddCustomLinesToCarInspection(carInspectionModel, carInspection);
                    //this.SaveCarInspectionsCustomAndOversizedDentsLines(carInspection, carInspectionModel);

                    carInspection.Aluminium = carInspectionModel.Aluminium;

                    var isBumper = carInspectionModel.Name != PartOfBody.FrontBumper &&
                                carInspectionModel.Name != PartOfBody.RearBumper;
                    carInspection.AverageSize = isBumper ? carInspectionModel.AverageSize : (AverageSize) (-1);
                    carInspection.DentsAmount = isBumper ? carInspectionModel.DentsAmount : (TotalDents) (-1);

                    //carInspection.Company = this.userStorage.Get().Company;
                    carInspection.CorrosionProtection = carInspectionModel.CorrosionProtection;
                    carInspection.DentsCost = carInspectionModel.DentsCost;

                    carInspection.DoubleMetal = carInspectionModel.DoubleMetal;
                    carInspection.PartsTotal = carInspectionModel.PartsTotal;
                    carInspection.OptionsPercent = carInspectionModel.OptionsPercent;

                    carInspection.OversizedRoof = carInspectionModel.OversizedRoof;
                    carInspection.Name = carInspectionModel.Name;
                    carInspection.PriorDamage = string.IsNullOrWhiteSpace(carInspectionModel.PriorDamage)
                                                    ? carInspectionModel.PriorDamage
                                                    : carInspectionModel.PriorDamage.Replace("&lt;", "<");
                    //carInspection.HasAlert = carInspectionModel.HasAlert.HasValue && carInspectionModel.HasAlert.Value;

                    carInspection.CorrosionProtectionCost = carInspectionModel.CorrosionProtectionCost;
                    carInspection.Estimate = estimate;

                    carInspection.AluminiumAmount = carInspection.CalculateAluminiumSum();
                    carInspection.OversizedRoofAmount = carInspection.OversizedRoofSum();
                    carInspection.DoubleMetallAmount = carInspection.CalculateDoubleMetallSum();
                    carInspection.AdditionalPercentsAmount = carInspection.AdditionalPercentsSum();

                    this.carInspections.Save(carInspection);
                    this.AddEffortsToCarInspection(carInspectionModel, carInspection);
                    inspectTemp.Add(carInspection);
                }

                foreach (var carInspection in inspectTemp)
                {
                    estimate.CarInspections.Add(carInspection);
                }
            }

            //// Remove estimate with partstotal = 0
            foreach (var carInspection in inspectTempRemove)
            {
                var chosenEffortItemstemp = carInspection.ChosenEffortItems.ToList();
                var customLinesTemp = carInspection.CustomLines.ToList();

                foreach (var item in chosenEffortItemstemp)
                {
                    item.CarInspection = null;
                    this.chosenEffortItems.Remove(item);
                    carInspection.ChosenEffortItems.Remove(item);
                }

                foreach (var line in customLinesTemp)
                {
                    this.customLines.Remove(line);
                    carInspection.CustomLines.Remove(line);
                }

                ////Nulling quick type estimate for status Normal
                //if (carInspection.QuickCost != null)
                //{
                //    carInspection.Aluminium = false;
                //    carInspection.DoubleMetal = false;
                //    carInspection.OversizedRoof = false;
                //    carInspection.CorrosionProtection = false;
                //    carInspection.AverageSize = (AverageSize)(-1);
                //    carInspection.DentsAmount = (TotalDents)(-1);
                //    carInspection.DentsCost = 0;
                //    continue;
                //}

                this.carInspections.Remove(carInspection);
                estimate.CarInspections.Remove(carInspection);
            }
        }

        private void AddCarInspections(ApiEstimateModel model, Estimate estimate, long? oldCustomerId, long? oldMatrixId)
        {
            switch (estimate.Type)
            {
                case EstimateType.ExtraQuick:
                    return;
                case EstimateType.Quick:
                    AddQuickCarInspections(model, estimate, oldCustomerId, oldMatrixId);
                    return;
                case EstimateType.Normal:
                    AddNormalCarInspections(model, estimate, oldCustomerId, oldMatrixId);
                    return;
            }
        }

        private void AddEffortsToCarInspection(ApiCarInspectionModel model, CarInspection carInspection)
        {
            var chosenNew = new HashedSet<ChosenEffortItem>();
            var chosenRemove = new List<ChosenEffortItem>();

            foreach (var item in model.ChoosenEffortItems)
            {
                var effort = carInspection.ChosenEffortItems.IsEmpty
                                 ? new ChosenEffortItem(true)
                                 : carInspection.ChosenEffortItems.SingleOrDefault(x => x.Id == item.Id)
                                    ?? new ChosenEffortItem(true);

                effort.Operations = item.Operation == Operations.R_I ? Operations.R_I : Operations.R_R;
                effort.EffortItem = effortItems.Get(Convert.ToInt64(item.EffortId));
                effort.Choosed = true;
                effort.ChosenEffortType = item.EffortType;
                var hourstemp = effort.Operations == Operations.R_I
                                    ? Convert.ToDouble(effort.EffortItem.HoursR_I)
                                    : Convert.ToDouble(effort.EffortItem.HoursR_R);

                effort.Hours = Math.Round((double)(Convert.ToDouble(item.Hours) != hourstemp ? item.Hours : hourstemp), 2);

                this.chosenEffortItems.Save(effort);
                chosenNew.Add(effort);

                effort.CarInspection = carInspection;
            }

            foreach (var chosenEffortItem in chosenNew)
            {
                carInspection.ChosenEffortItems.Add(chosenEffortItem);
            }

            if (chosenNew.Count != 0)
            {
                chosenRemove.AddRange(carInspection.ChosenEffortItems.Where(item => !chosenNew.Contains(item)));
            }
            else
            {
                chosenRemove = carInspection.ChosenEffortItems.ToList();
            }

            foreach (var chosenEffortItem in chosenRemove)
            {
                carInspection.ChosenEffortItems.Remove(chosenEffortItem);
                this.chosenEffortItems.Remove(chosenEffortItem);
            }
        }

        private void AddAffiliate(ApiEstimateModel model, Estimate estimate)
        {
            estimate.Affiliate = model.LocationId != null
                ? affiliates.SingleOrDefault(x => x.Id == model.LocationId.Value)
                : null;
        }

        private void AddCustomLinesToCarInspection(ApiCarInspectionModel model, CarInspection carInspection)
        {
            var carInspectLineTemp = new HashedSet<CustomCarInspectionLine>();
            var carInspectLineRemove = new List<CustomCarInspectionLine>();
            var carLines = carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).Cast<CustomCarInspectionLine>().ToList();
            var overLines = carInspection.CustomLines.Where(x => x is OversizedDentsLine).Cast<OversizedDentsLine>().ToList();
            var overLineTemp = new HashedSet<OversizedDentsLine>();
            var overLineRemove = new List<OversizedDentsLine>();

            foreach (var line in model.CustomLines)
            {
                //// && line.Cost != null
                if (string.IsNullOrEmpty(line.Name))
                {
                    continue;
                }

                var carinspect = carLines.Count == 0
                                     ? new CustomCarInspectionLine(true)
                                     : carLines.SingleOrDefault(x => x.Id == line.Id)
                                        ?? new CustomCarInspectionLine(true);

                CustomAutomapper<ApiCustomFieldModel, CustomCarInspectionLine>.Map(line, carinspect);
                //carinspect.Name = carinspect.Name.Replace("&lt;", "<");

                this.carInspectionLines.Save(carinspect);
                carInspectLineTemp.Add(carinspect);
                carinspect.CarInspection = carInspection;
            }

            foreach (var customCarInspectionLine in carInspectLineTemp)
            {
                carInspection.CustomLines.Add(customCarInspectionLine);
            }

            if (carInspectLineTemp.Count != 0)
            {
                carInspectLineRemove.AddRange(carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).Cast<CustomCarInspectionLine>().ToList().Where(x => !carInspectLineTemp.Contains(x)));
            }
            else
            {
                carInspectLineRemove = carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).Cast<CustomCarInspectionLine>().ToList();
            }

            foreach (var l in carInspectLineRemove)
            {
                carInspection.CustomLines.Remove(l);
                this.carInspectionLines.Remove(l);
            }

            if (model.OversizedDents.Cost != 0.0 && model.OversizedDents.Count != 0)
            {
                var line = !overLines.Any()
                                     ? new OversizedDentsLine(true)
                                     : overLines.SingleOrDefault(x => x.Id == model.OversizedDents.Id)
                                       ?? new OversizedDentsLine(true);
                line.CarInspection = carInspection;
                //line.Company = this.userStorage.Get().Company;
                line.Cost = model.OversizedDents.Cost;
                
                line.Name = model.OversizedDents.Count.ToString();

                this.oversizedDentsLines.Save(line);
                overLineTemp.Add(line);
            }

            foreach (var line in overLineTemp)
            {
                carInspection.CustomLines.Add(line);
            }

            if (overLineTemp.Count != 0)
            {
                overLineRemove.AddRange(carInspection.CustomLines.Where(x => x is OversizedDentsLine).Cast<OversizedDentsLine>().ToList().Where(x => !overLineTemp.Contains(x)));
            }
            else
            {
                overLineRemove = carInspection.CustomLines.Where(x => x is OversizedDentsLine).Cast<OversizedDentsLine>().ToList();
            }

            foreach (var l in overLineRemove)
            {
                carInspection.CustomLines.Remove(l);
                this.oversizedDentsLines.Remove(l);
            }
        }

        #endregion
    }
}