using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using Iesi.Collections.Generic;
using PDR.Domain.Model;
using PDR.Domain.Model.CustomLines;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Photos;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Automapper;
using PDR.Domain.Services.TempImageStorage;
using PDR.Web.Areas.Estimator.Models.Estimates;
using PDR.Web.Areas.Wholesaler.Models;
using SmartArch.Data;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Wholesaler.Managers
{
    public class SaveEstimateManager
    {
        private readonly IRepositoryFactory repositoryFactory;

        private readonly ITempImageManager tempImageManager;

        public SaveEstimateManager(IRepositoryFactory repositoryFactory, ITempImageManager tempImageManager)
        {
            this.repositoryFactory = repositoryFactory;
            this.tempImageManager = tempImageManager;
        }

        public Estimate Save(WholesalerEstimateModel wholesalerEstimateModel, WholesaleCustomer currentCustomer)
        {
            Employee employee =
                           this.repositoryFactory.CreateForCompany<Employee>()
                               .Single(x => x.Id == wholesalerEstimateModel.Employee.EmployeeId);

            Estimate estimate=new Estimate(true, employee);

            estimate.Signature = wholesalerEstimateModel.Signature;
            estimate.EstimateStatus = wholesalerEstimateModel.Status;
            estimate.Type = wholesalerEstimateModel.Type;

            // fill customer
            Matrix matrix = null;
            Insurance insurance = null;
          
                        matrix =
                            this.repositoryFactory.CreateForCompany<Matrix>().Single(
                                x => x.Id == wholesalerEstimateModel.MatrixId);
                  
                        estimate.WorkByThemselve = currentCustomer.WorkByThemselve;

                        insurance = new Insurance(true, employee);
                        CustomAutomapper<InsuranceModel, Insurance>.Map(wholesalerEstimateModel.Insurance, insurance);
                        insurance.CompanyName = wholesalerEstimateModel.Insurance.CompanyName.Replace("&lt;", "<");
            
           this.repositoryFactory.CreateNative<Insurance>().Save(insurance);
           estimate.Insurance = insurance;
           insurance.Estimate = estimate;

            currentCustomer.Estimates.Add(estimate);
            this.repositoryFactory.CreateNative<Customer>().Save(currentCustomer);
            estimate.Customer = currentCustomer;


            if (matrix == null)
            {
                matrix = this.repositoryFactory.CreateForCompany<DefaultMatrix>().Single();
            }

           this.repositoryFactory.CreateNative<Matrix>().Save(matrix);
            estimate.Matrix = matrix;

            ////fill carInfo
            this.SaveCarInfo(estimate, wholesalerEstimateModel);

            this.SavePhotos(estimate, wholesalerEstimateModel);

            estimate.Employee = employee;

            this.SaveEstimateCustomLines(estimate, wholesalerEstimateModel);

            this.SaveCarInspections(estimate, wholesalerEstimateModel, employee);

            estimate.PriorDamages = string.IsNullOrWhiteSpace(wholesalerEstimateModel.CarInspectionsModel.PriorDamages)
               ? wholesalerEstimateModel.CarInspectionsModel.PriorDamages
               : wholesalerEstimateModel.CarInspectionsModel.PriorDamages.Replace("&lt;", "<");

            //// remove carImage for PDF
            if(estimate.CarImage != null)
            {
                var id = estimate.CarImage.Id;
                estimate.CarImage = null;
                var img = repositoryFactory.CreateForCompany<CarImage>().Get(id);
                if (img != null)
                {
                    repositoryFactory.CreateForCompany<CarImage>().Remove(img);
                }
            }
            ////calculates
            estimate.SaveCalculation();
            ////end calculates

            return estimate;
        }

        private void SaveCarInfo(Estimate estimate, WholesalerEstimateModel wholesalerEstimateModel)
        {
            ////fill carInfo
            var car = estimate.Car ?? new Car(true);
            CustomAutomapper<CarInfoModel, Car>.Map(wholesalerEstimateModel.CarInfo, car);
            this.repositoryFactory.CreateNative<Car>().Save(car);
            estimate.Car = car;
        }

        private void SavePhotos(Estimate estimate, WholesalerEstimateModel wholesalerEstimateModel)
        {
            var storedPhotosIds = wholesalerEstimateModel.StoredPhotos.Select(x => long.Parse(x.Id));
            var removedPhotos = estimate.Photos.Where(x => !storedPhotosIds.Contains(x.Id)).ToList();
            for (int i = 0; i < removedPhotos.Count; i++)
            {
                removedPhotos[i].Estimate = null;
                estimate.Photos.Remove(removedPhotos[i]);
                this.repositoryFactory.CreateForCompany<Photo>().Remove(removedPhotos[i]);
            }

            // fill upload photos
            for (int i = 0; i < wholesalerEstimateModel.UploadPhotos.Count; i++)
            {
                var imgInfo = this.tempImageManager.Get(wholesalerEstimateModel.UploadPhotos[i].Id);
                var photo = new CarPhoto(
                                         HostingEnvironment.MapPath(imgInfo.FullSizeUrl),
                                         HostingEnvironment.MapPath(imgInfo.ThumbnailUrl),
                                         imgInfo.ContentType, true) { Estimate = estimate };
                estimate.Photos.Add(photo);
                this.tempImageManager.Remove(wholesalerEstimateModel.UploadPhotos[i].Id);
            }
        }

        private void SaveEstimateCustomLines(Estimate estimate, WholesalerEstimateModel wholesalerEstimateModel)
        {
            var estimateCustomLinetemp = new HashedSet<EstimateCustomLine>();
            var estimateCustomLineRemove = new List<EstimateCustomLine>();

            for (int i = 0; i < wholesalerEstimateModel.CarInspectionsModel.EstimateCustomLines.Count; i++)
            {
                //// && t.Cost != null
                if (!string.IsNullOrEmpty(wholesalerEstimateModel.CarInspectionsModel.EstimateCustomLines[i].Name))
                {
                    var estimateCustomLine = estimate.CustomEstimateLines.IsEmpty
                                             ? new EstimateCustomLine(true)
                                             : estimate.CustomEstimateLines.ToList().SingleOrDefault(x => x.Id == wholesalerEstimateModel.CarInspectionsModel.EstimateCustomLines[i].Id) ?? new EstimateCustomLine(true);
                    CustomAutomapper<EstimateCustomLineModel, EstimateCustomLine>.Map(wholesalerEstimateModel.CarInspectionsModel.EstimateCustomLines[i], estimateCustomLine);
                    estimateCustomLine.Name = estimateCustomLine.Name.Replace("&lt;", "<");

                    this.repositoryFactory.CreateForCompany<EstimateCustomLine>().Save(estimateCustomLine);
                    estimateCustomLinetemp.Add(estimateCustomLine);
                    estimateCustomLine.Estimate = estimate;
                }
            }
            var estimateCustomLinetempArr = estimateCustomLinetemp.ToArray();
            for (int i = 0; i < estimateCustomLinetempArr.Length; i++)
            {
                estimate.CustomEstimateLines.Add(estimateCustomLinetempArr[i]);
            }

            if (estimateCustomLinetemp.Count != 0)
            {
                estimateCustomLineRemove.AddRange(estimate.CustomEstimateLines.Where(item => !estimateCustomLinetemp.Contains(item)));
            }
            else
            {
                estimateCustomLineRemove = estimate.CustomEstimateLines.ToList();
            }
            for (int i = 0; i < estimateCustomLineRemove.Count; i++)
            {
                estimate.CustomEstimateLines.Remove(estimateCustomLineRemove[i]);
                this.repositoryFactory.CreateForCompany<EstimateCustomLine>().Remove(estimateCustomLineRemove[i]);
            }
        }

        private void SaveChosenEffortItems(CarInspection carInspection, CarInspectionsInfo carInspectionsInfo, Employee employee)
        {
            var chosenNew = new HashedSet<ChosenEffortItem>();
            var chosenRemove = new List<ChosenEffortItem>();

            var effortItems = carInspectionsInfo.EffortItems.ToArray();
            for (int i = 0; i < effortItems.Length; i++)
            {
                if (effortItems[i].Choosed == false)
                {
                    continue;
                }
                if (effortItems[i].Choosed == true && !effortItems[i].Hours.HasValue)
                {
                    continue;
                }

                var effort = carInspection.ChosenEffortItems.IsEmpty
                                 ? new ChosenEffortItem(true)
                                 : carInspection.ChosenEffortItems.ToList().SingleOrDefault(
                                     x => x.EffortItem.Id == effortItems[i].Id) ??
                                   new ChosenEffortItem(true);
                effort.Operations = effortItems[i].Operations == Operations.R_I ? Operations.R_I : Operations.R_R;
                effort.EffortItem =
                    this.repositoryFactory.CreateForCompany<EffortItem>().Get(Convert.ToInt64(effortItems[i].Id));
                effort.Choosed = Convert.ToBoolean(effortItems[i].Choosed);
                effort.ChosenEffortType = effortItems[i].EffortType;
                effort.Company = employee.Company;
                var hourstemp = effort.Operations == Operations.R_I
                                    ? Convert.ToDouble(effortItems[i].HoursR_I)
                                    : Convert.ToDouble(effortItems[i].HoursR_R);
                effort.Hours = Math.Round((double)(Convert.ToDouble(effortItems[i].Hours) != hourstemp ? effortItems[i].Hours : hourstemp), 2);

                this.repositoryFactory.CreateForCompany<ChosenEffortItem>().Save(effort);
                chosenNew.Add(effort);

                effort.CarInspection = carInspection;
            }

            var chosenNewArr = chosenNew.ToArray();
            for (int i = 0; i < chosenNewArr.Length; i++)
            {
                carInspection.ChosenEffortItems.Add(chosenNewArr[i]);
            }

            if (chosenNew.Count != 0)
            {
                chosenRemove.AddRange(carInspection.ChosenEffortItems.Where(item => !chosenNew.Contains(item)));
            }
            else
            {
                chosenRemove = carInspection.ChosenEffortItems.ToList();
            }

            for (int i = 0; i < chosenRemove.Count; i++)
            {
                carInspection.ChosenEffortItems.Remove(chosenRemove[i]);
                this.repositoryFactory.CreateForCompany<ChosenEffortItem>().Remove(chosenRemove[i]);
            }
        }

        private void SaveCarInspectionsCustomAndOversizedDentsLines(CarInspection carInspection, CarInspectionsInfo carInspectionsInfo, Employee employee)
        {
            var carInspectLineTemp = new HashedSet<CustomCarInspectionLine>();
            var carInspectLineRemove = new List<CustomCarInspectionLine>();
            var carLines = carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).Cast<CustomCarInspectionLine>().ToList();
            var overLines = carInspection.CustomLines.Where(x => x is OversizedDentsLine).Cast<OversizedDentsLine>().ToList();
            var overLineTemp = new HashedSet<OversizedDentsLine>();
            var overLineRemove = new List<OversizedDentsLine>();

            var customCarInspectionLines = carInspectionsInfo.CustomCarInspectionLines.ToArray();
            for (int i = 0; i < customCarInspectionLines.Length; i++)
            {
                //// && line.Cost != null
                if (!string.IsNullOrEmpty(customCarInspectionLines[i].Name))
                {
                    var carinspect = carLines.Count == 0
                                         ? new CustomCarInspectionLine(true)
                                         : carLines.SingleOrDefault(x => x.Id == customCarInspectionLines[i].Id) ?? new CustomCarInspectionLine(true);
                    CustomAutomapper<CustomCarInspectionLineModel, CustomCarInspectionLine>.Map(customCarInspectionLines[i], carinspect);
                    carinspect.Name = carinspect.Name.Replace("&lt;", "<");

                    this.repositoryFactory.CreateForCompany<CustomCarInspectionLine>().Save(carinspect);
                    carInspectLineTemp.Add(carinspect);
                    carinspect.CarInspection = carInspection;
                }
            }

            var carInspectLineTempArr = carInspectLineTemp.ToArray();
            for (int i = 0; i < carInspectLineTempArr.Length; i++)
            {
                carInspection.CustomLines.Add(carInspectLineTempArr[i]);
            }

            if (carInspectLineTemp.Count != 0)
            {
                carInspectLineRemove.AddRange(carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).Cast<CustomCarInspectionLine>().ToList().Where(x => !carInspectLineTemp.Contains(x)));
            }
            else
            {
                carInspectLineRemove = carInspection.CustomLines.Where(x => x is CustomCarInspectionLine).Cast<CustomCarInspectionLine>().ToList();
            }
            for (int i = 0; i < carInspectLineRemove.Count; i++)
            {
                carInspection.CustomLines.Remove(carInspectLineRemove[i]);
                this.repositoryFactory.CreateForCompany<CustomCarInspectionLine>().Remove(carInspectLineRemove[i]);
            }

            if (carInspectionsInfo.AmountOversizedDents != null && carInspectionsInfo.OversizedDents != null)
            {
                var line = overLines.Count() == 0
                                     ? new OversizedDentsLine(true)
                                     : overLines.SingleOrDefault(x => x.Id == carInspectionsInfo.OversizedDentsId) ?? new OversizedDentsLine(true);
                line.CarInspection = carInspection;
                line.Company = employee.Company;
                if (carInspectionsInfo.OversizedDents.HasValue)
                {
                    line.Cost = carInspectionsInfo.OversizedDents.Value;
                }

                line.Name = carInspectionsInfo.AmountOversizedDents.ToString();

                this.repositoryFactory.CreateForCompany<OversizedDentsLine>().Save(line);
                overLineTemp.Add(line);
            }

            var overLineTempArr = overLineTemp.ToArray();
            for (int i = 0; i < overLineTempArr.Length; i++)
            {
                carInspection.CustomLines.Add(overLineTempArr[i]);
            }

            if (overLineTemp.Count != 0)
            {
                overLineRemove.AddRange(carInspection.CustomLines.Where(x => x is OversizedDentsLine).Cast<OversizedDentsLine>().ToList().Where(x => !overLineTemp.Contains(x)));
            }
            else
            {
                overLineRemove = carInspection.CustomLines.Where(x => x is OversizedDentsLine).Cast<OversizedDentsLine>().ToList();
            }

            for (int i = 0; i < overLineRemove.Count; i++)
            {
                carInspection.CustomLines.Remove(overLineRemove[i]);
                this.repositoryFactory.CreateForCompany<OversizedDentsLine>().Remove(overLineRemove[i]);
            }
        }

        private void SaveCarInspections(Estimate estimate, WholesalerEstimateModel wholesalerEstimateModel, Employee employee)
        {
            var inspectTemp = new HashedSet<CarInspection>();
            var inspectTempRemove = new HashedSet<CarInspection>();
            if (wholesalerEstimateModel.Type != EstimateType.Normal)
            {
                return;
            }

            if (wholesalerEstimateModel.CarInfo.Type == VehicleTypes.Car)
            {
                inspectTempRemove.AddAll(
                    estimate.CarInspections.Where(
                        x => x.Name == PartOfBody.LCabCorner || x.Name == PartOfBody.RCabCorner).ToList());
            }

            var carInspections = wholesalerEstimateModel.CarInspectionsModel.CarInspections.Where(x => x.IsChanges == "1").ToArray();
            for (int i = 0; i < carInspections.Length; i++)
            {
                if (carInspections[i].PartsTotal == 0 && !carInspections[i].CorrosionProtection)
                {
                    var inspect =
                        this.repositoryFactory.CreateForCompany<CarInspection>().SingleOrDefault(
                            x => x.Id == Convert.ToInt64(carInspections[i].Id));
                    if (inspect != null)
                    {
                        inspectTempRemove.Add(inspect);
                    }

                    continue;
                }

                var carInspection = estimate.CarInspections.IsEmpty
                                        ? new CarInspection(true)
                                        : estimate.CarInspections.ToList().SingleOrDefault(
                                            x => x.Id == carInspections[i].Id) ?? new CarInspection(true);

                carInspection.Estimate = estimate;

                this.SaveCarInspectionsCustomAndOversizedDentsLines(carInspection, carInspections[i], employee);

                carInspection.Aluminium = carInspections[i].Aluminium;
                if (carInspections[i].Name != PartOfBody.FrontBumper &&
                    carInspections[i].Name != PartOfBody.RearBumper)
                {
                    carInspection.AverageSize = carInspections[i].AverageSize;
                    carInspection.DentsAmount = carInspections[i].TotalDents;
                }
                else
                {
                    carInspection.AverageSize = (AverageSize)(-1);
                    carInspection.DentsAmount = (TotalDents)(-1);
                }

                carInspection.Company = employee.Company;
                carInspection.CorrosionProtection = carInspections[i].CorrosionProtection;

                carInspection.DentsCost = carInspections[i].DentsCost.HasValue ? carInspections[i].DentsCost.Value : 0;

                carInspection.DoubleMetal = carInspections[i].DoubleMetal;

                carInspection.PartsTotal = carInspections[i].PartsTotal.HasValue ? carInspections[i].PartsTotal.Value : 0;

                carInspection.OptionsPercent = carInspections[i].OptionsPersent.HasValue ? carInspections[i].OptionsPersent.Value : 0;

                carInspection.OversizedRoof = carInspections[i].OversizedRoof;
                carInspection.Name = carInspections[i].Name;
                carInspection.PriorDamage = string.IsNullOrWhiteSpace(carInspections[i].PriorDamage) ? carInspections[i].PriorDamage : carInspections[i].PriorDamage.Replace("&lt;", "<");
                carInspection.HasAlert = carInspections[i].HasAlert.HasValue && carInspections[i].HasAlert.Value;

                carInspection.CorrosionProtectionCost = carInspections[i].CorrosionProtectionCost.HasValue ? carInspections[i].CorrosionProtectionCost.Value : 0;

                carInspection.AluminiumAmount = carInspection.CalculateAluminiumSum();
                carInspection.OversizedRoofAmount = carInspection.OversizedRoofSum();
                carInspection.DoubleMetallAmount = carInspection.CalculateDoubleMetallSum();
                carInspection.AdditionalPercentsAmount = carInspection.AdditionalPercentsSum();

                this.repositoryFactory.CreateForCompany<CarInspection>().Save(carInspection);
                this.SaveChosenEffortItems(carInspection, carInspections[i], employee);
                inspectTemp.Add(carInspection);
            }

            var inspectTempArr = inspectTemp.ToArray();
            for (int i = 0; i < inspectTempArr.Length; i++)
            {
                estimate.CarInspections.Add(inspectTempArr[i]);
            }

            //// Remove estimate with partstotal = 0
            var inspectTempRemoveArr = inspectTempRemove.ToArray();
            for (int i = 0; i < inspectTempRemoveArr.Length; i++)
            {
                var chosenEffortItemstemp = inspectTempRemoveArr[i].ChosenEffortItems.ToList();
                var customLinesTemp = inspectTempRemoveArr[i].CustomLines.ToList();

                for (int j = 0; j < chosenEffortItemstemp.Count; j++)
                {
                    chosenEffortItemstemp[j].CarInspection = null;
                    this.repositoryFactory.CreateForCompany<ChosenEffortItem>().Remove(chosenEffortItemstemp[j]);
                    inspectTempRemoveArr[i].ChosenEffortItems.Remove(chosenEffortItemstemp[j]);
                }

                for (int j = 0; j < customLinesTemp.Count; j++)
                {
                    this.repositoryFactory.CreateForCompany<CustomLine>().Remove(customLinesTemp[j]);
                    inspectTempRemoveArr[i].CustomLines.Remove(customLinesTemp[j]);
                }

                ////Nulling quick type estimate for status Normal
                if (inspectTempRemoveArr[i].QuickCost != null)
                {
                    inspectTempRemoveArr[i].Aluminium = false;
                    inspectTempRemoveArr[i].DoubleMetal = false;
                    inspectTempRemoveArr[i].OversizedRoof = false;
                    inspectTempRemoveArr[i].CorrosionProtection = false;
                    inspectTempRemoveArr[i].AverageSize = (AverageSize)(-1);
                    inspectTempRemoveArr[i].DentsAmount = (TotalDents)(-1);
                    inspectTempRemoveArr[i].DentsCost = 0;
                    continue;
                }

                this.repositoryFactory.CreateForCompany<CarInspection>().Remove(inspectTempRemoveArr[i]);
                estimate.CarInspections.Remove(inspectTempRemoveArr[i]);
            }
        }
    }
}