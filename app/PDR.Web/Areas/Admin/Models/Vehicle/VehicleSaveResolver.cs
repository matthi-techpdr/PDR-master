using System;
using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using SmartArch.Data;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleSaveResolver
    {
        private readonly ICompanyRepository<CarSectionsPrice> sectionsPricesRepository;

        private readonly ICompanyRepository<EffortItem> effortItemsRepository; 

        public VehicleSaveResolver(IRepositoryFactory repositoryFactory, bool isNew = false)
        {
            this.sectionsPricesRepository = repositoryFactory.CreateForCompany<CarSectionsPrice>();
            this.effortItemsRepository = repositoryFactory.CreateForCompany<EffortItem>();
            this.IsNew = isNew;
        }

        public void Save(CarModel carModel, VehicleModel vehicleModel)
        {
            carModel.Make = vehicleModel.Info.Make;
            carModel.Model = vehicleModel.Info.Model;
            carModel.YearFrom = vehicleModel.Info.Make.ToLower() == "DefaultCar".ToLower() ? 1756 : Convert.ToInt32(vehicleModel.Info.YearFrom);
            carModel.YearTo = vehicleModel.Info.Make.ToLower() == "DefaultCar".ToLower() ? 1756 : Convert.ToInt32(vehicleModel.Info.YearTo);
            carModel.Type = vehicleModel.Info.VehicleType;

            this.SaveSections(carModel, vehicleModel);
        }

        public bool IsNew { get; private set; }

        private static bool CheckEffortValue(VehicleEffortModel model)
        {
            if (model.Id.HasValue)
            {
                return false;
            }

            var ri = model.HasR_I ? string.IsNullOrWhiteSpace(model.HoursR_I) ? 0.0 : Convert.ToDouble(model.HoursR_I) : 0.0;
            var rr = model.HasR_R ? string.IsNullOrWhiteSpace(model.HoursR_R) ? 0.0 : Convert.ToDouble(model.HoursR_R) : 0.0;

            return ri == 0.0 && rr == 0.0;
        }

        private void SaveEffort(CarSectionsPrice part, VehicleSectionModel model)
        {
            foreach (var effort in model.EffortItems)
            {
                if (CheckEffortValue(effort))
                {
                    continue;
                }

                var item = part.EffortItems.IsEmpty
                                 ? new EffortItem(true)
                                 : part.EffortItems.ToList().SingleOrDefault(x => x.Id == effort.Id) ?? new EffortItem(true);

                item.CarSectionsPrices = part;
                item.Name = effort.Name;
                if (effort.HasR_I)
                {
                    item.HoursR_I = string.IsNullOrWhiteSpace(effort.HoursR_I)
                                              ? 0.0
                                              : Convert.ToDouble(effort.HoursR_I);
                }

                if (effort.HasR_R)
                {
                    item.HoursR_R = string.IsNullOrWhiteSpace(effort.HoursR_R)
                                              ? 0.0
                                              : Convert.ToDouble(effort.HoursR_R);
                }

                this.effortItemsRepository.Save(item);
                part.EffortItems.Add(item);
            }
        }

        private void SaveSections(CarModel carModel, VehicleModel vehicleModel)
        {
            var removeSection = new List<CarSectionsPrice>();
            foreach (var section in vehicleModel.SectionsModel.Sections)
            {
                if (vehicleModel.Info.VehicleType == VehicleTypes.Car
                    && vehicleModel.Info.Make.ToLower() != "defaultcar"
                    && (section.Name == PartOfBody.LCabCorner || section.Name == PartOfBody.RCabCorner))
                {
                    if (this.IsNew) continue;

                    if(!section.Id.HasValue) continue;
                    
                    var sec = sectionsPricesRepository.Get(section.Id.Value);
                    
                    if(sec != null)
                    {
                        removeSection.Add(sec);
                    }

                    continue;
                }

                var price = string.IsNullOrWhiteSpace(section.Price) ? 0.0 : Convert.ToDouble(section.Price);
                if (price == 0.0 && section.EffortItems.All(CheckEffortValue))
                {
                    continue;
                }

                var part = carModel.CarParts.IsEmpty
                                   ? new CarSectionsPrice(true)
                                   : carModel.CarParts.ToList().SingleOrDefault(x => x.Id == section.Id) ??
                                     new CarSectionsPrice(true);

                part.CarModel = carModel;
                part.Name = section.Name;
                part.NewSectionPrice = price;
                this.sectionsPricesRepository.Save(part);
                this.SaveEffort(part, section);

                carModel.CarParts.Add(part);
            }

            foreach (var carSectionsPrice in removeSection)
            {
                carModel.CarParts.Remove(carSectionsPrice);
                this.sectionsPricesRepository.Remove(carSectionsPrice);
            }
        }
    }
}