using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Resources.Web.Area.Admin.Vehicle;

using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Admin.Models.Vehicle
{
    public class VehicleModel
    {
        private static readonly IRepositoryFactory RepositoryFactory =
            ServiceLocator.Current.GetInstance<IRepositoryFactory>();

        public VehicleModel()
        {
            this.Info = new VehicleInfoModel();
            this.SectionsModel = new VehicleSectionsModel();
            this.GetDefaultVehicle();
            
            this.Info.VehicleTypes = GetVehicleTypes(null);
            this.Info.AllMakeNames = GetMakeNames(null);
        }

        public VehicleModel(CarModel carModel, string action) : this()
        {
            if (carModel == null)
            {
                return;
            }
            
            switch (action)
            {
                case "New":
                case "Duplicate":
                    this.Info.Id = string.Empty;
                    this.Info.Make = string.Empty;
                    this.Info.Model = string.Empty;
                    this.Info.YearFrom = string.Empty;
                    this.Info.YearTo = string.Empty;
                    if (carModel.Model.ToLower() != "defaultcar")
                    {
                        this.Info.VehicleTypes = GetVehicleTypes(carModel.Type.ToString());
                    }

                    break;
                case "Edit":
                case "View":
                    this.Info.Id = carModel.Id.ToString();
                    this.Info.Make = carModel.Make;
                    this.Info.Model = carModel.Model;
                    this.Info.YearFrom = carModel.Model.ToLower() != "defaultcar" ? carModel.YearFrom.ToString() : "1901";
                    this.Info.YearTo = carModel.Model.ToLower() != "defaultcar" ? carModel.YearTo.ToString() : "1901";
                    if (carModel.Model.ToLower() == "defaultcar")
                    {
                        this.Info.VehicleTypes = GetDefaultVehicleTypes(this.Info.VehicleTypes);
                    }
                    else
                    {
                        this.Info.VehicleTypes = GetVehicleTypes(carModel.Type.ToString());
                    }

                    this.Info.AllMakeNames = GetMakeNames(carModel.Make);
                    break;
                default:
                    break;
            }

            this.SetSections(carModel, action);
        }

        public VehicleInfoModel Info { get; set; }

        public VehicleSectionsModel SectionsModel { get; set; }

        public VehicleSectionsModel DefaultSectionsModel { get; private set; }

        public IDictionary<PartOfBody, string> SectionsNames
        {
            get { return GetFullPartsName(); }
        }

        private static IList<SelectListItem> GetDefaultVehicleTypes(IList<SelectListItem> list)
        {
            list.Insert(0, new SelectListItem { Text = string.Empty, Value = "-1", Selected = true });
            return list;
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

        private static IDictionary<PartOfBody, string> GetFullPartsName()
        {
            return new Dictionary<PartOfBody, string>
                                   {
                                       { PartOfBody.Hood, "Hood" },
                                       { PartOfBody.Roof, "Roof" },
                                       { PartOfBody.DeckLid, "Deck lid" },
                                       { PartOfBody.RFender, "Right fender" },
                                       { PartOfBody.LFender, "Left fender" },
                                       { PartOfBody.RFDoor, "Right front door" },
                                       { PartOfBody.RRDoor, "Right rear door" },
                                       { PartOfBody.RCabCorner, "Right cab corner" },
                                       { PartOfBody.LFDoor, "Left front door" },
                                       { PartOfBody.LRDoor, "Left rear door" },
                                       { PartOfBody.LCabCorner, "Left cab corner" },
                                       { PartOfBody.RQuarter, "Right quarter" },
                                       { PartOfBody.LQuarter, "Left quarter" },
                                       { PartOfBody.MetalSunroof, "Metal sunroof" },
                                       { PartOfBody.LtRoofRail, "Left roof rail" },
                                       { PartOfBody.RtRoofRail, "Right roof rail" },                           
                                       { PartOfBody.Other, "Cowl/Other" },
                                       { PartOfBody.FrontBumper, "Front bumper" },
                                       { PartOfBody.RearBumper, "Rear bumper" }
                                   };
        }

        private static IList<SelectListItem> GetMakeNames(string currentName = null)
        {
            var makeNames = RepositoryFactory.CreateForCompany<CarModel>()
                .Where(x => x.Make.ToLower() != "DefaultCar".ToLower())
                .Select(x => x.Make)
                .Distinct()
                .ToList();


            var defaultMakes = VehicleMakeNames.Makes.Split(',').ToList();
            makeNames = makeNames.Union(defaultMakes).Distinct().ToList();
            makeNames.Sort();

            var selectList = makeNames.Select(companyName => new SelectListItem { Text = companyName }).ToList();

            if (!string.IsNullOrWhiteSpace(currentName))
            {
                var option =
                    selectList.SingleOrDefault(
                        x => x.Text == currentName);
                if (option != null)
                {
                    option.Selected = true;
                }
                else
                {
                    selectList.Insert(0, new SelectListItem { Text = currentName, Selected = true });
                }
            }
            else
            {
                selectList.Insert(0, new SelectListItem { Text = string.Empty, Value = string.Empty, Selected = true });
            }

            return selectList;
        }

        private void SetSections(CarModel model, string action)
        {
            for (var i = 0; i < this.DefaultSectionsModel.Sections.Count; i++)
            {
                var defaultSection = this.DefaultSectionsModel.Sections[i];
                if (action.ToLower() != "New".ToLower())
                {
                    var section = model.CarParts.SingleOrDefault(x => x.Name == defaultSection.Name);
                    if (section != null)
                    {
                        defaultSection.Price = section.NewSectionPrice > 0
                                                   ? section.NewSectionPrice.ToString("0.00")
                                                   : string.Empty;
                        defaultSection.Id = section.Id;

                        for (var j = 0; j < defaultSection.EffortItems.Count; j++)
                        {
                            var effort = defaultSection.EffortItems[j];
                            var item = section.EffortItems.SingleOrDefault(x => x.Name == effort.Name);
                            if (item != null)
                            {
                                effort.HasR_I = item.HoursR_I.HasValue;
                                effort.HasR_R = item.HoursR_R.HasValue;
                                effort.HoursR_I = item.HoursR_I.HasValue
                                                      ? item.HoursR_I.Value > 0
                                                            ? item.HoursR_I.Value.ToString("0.00")
                                                            : string.Empty
                                                      : null;
                                effort.HoursR_R = item.HoursR_R.HasValue
                                                      ? item.HoursR_R.Value > 0
                                                            ? item.HoursR_R.Value.ToString("0.00")
                                                            : string.Empty
                                                      : null;
                                effort.Id = item.Id;
                            }
                        }
                    }
                }

                this.SectionsModel.Sections.Add(defaultSection);
            }
        }

        private void GetDefaultVehicle()
        {
            var defaultVehicle = RepositoryFactory.CreateForCompany<CarModel>().SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());
            this.DefaultSectionsModel = new VehicleSectionsModel();
            foreach (var partName in GetFullPartsName())
            {
                var section = defaultVehicle.CarParts.SingleOrDefault(x => x.Name == partName.Key);
                if (section != null)
                {
                    var sectionModel = new VehicleSectionModel
                    {
                        Name = partName.Key,
                        FullName = partName.Value,
                        Price = string.Empty
                    };
                    foreach (var item in section.EffortItems)
                    {
                        sectionModel.EffortItems.Add(new VehicleEffortModel
                        {
                            HasR_I = item.HoursR_I.HasValue,
                            HasR_R = item.HoursR_R.HasValue,
                            HoursR_I = item.HoursR_I.HasValue ? string.Empty : null,
                            HoursR_R = item.HoursR_R.HasValue ? string.Empty : null,
                            //Id = item.Id,
                            Name = item.Name
                        });
                    }

                    this.DefaultSectionsModel.Sections.Add(sectionModel);
                }
            }
        }
    }
}