using System;
using System.Collections.Generic;
using System.Linq;

using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;

using PDR.Web.Areas.Estimator.Models.Estimates;

namespace PDR.Web.Areas.Common.Models
{
    public class EffortsDataModel
    {
        public static IList<SectionEffortItemViewModel> GetSectionEffortItemViewModels(CarModel car, CarModel defaultCar)
        {
            var data = new List<SectionEffortItemViewModel>();
            var parts = car.CarParts.OrderBy(x => x.Name).ToList();
            var defaultParts = defaultCar.CarParts.ToList();
            foreach (var carModel in Enum.GetValues(typeof(PartOfBody)).Cast<int>())
            {
                var part = parts.SingleOrDefault(x => (int)x.Name == carModel);
                var defaultsection = defaultParts.Single(x => (int)x.Name == carModel);
                data.Add(GetSectionEffortItemModel(part ?? defaultsection, defaultsection, part == null || car.Model == "DefaultCar"));
            }

            return data;
        }

        private static SectionEffortItemViewModel GetSectionEffortItemModel(CarSectionsPrice section, CarSectionsPrice defaultsection, bool defaultSection)
        {
            var defaultEffortItems = defaultsection.EffortItems.ToList();
            var effortItems = section.EffortItems.ToList();
            return new SectionEffortItemViewModel
            {
                Name = section.Name,
                Cost = section.NewSectionPrice,
                EffortItems = GetEffortItems(effortItems, defaultEffortItems, defaultSection)
            };
        }

        private static IList<EffortItemModel> GetEffortItems(IEnumerable<EffortItem> effortItems, IEnumerable<EffortItem> defaultEffortItems, bool defaultSection)
        {
            var effortItemsModel = new List<EffortItemModel>();
            foreach (var item in defaultEffortItems)
            {
                if (defaultSection)
                {
                    effortItemsModel.Add(GetEffortItemModel(item, ChosenEffortType.Default));
                }
                else
                {
                    var effortItem = effortItems.SingleOrDefault(x => x.Name == item.Name);
                    effortItemsModel.Add(GetEffortItemModel(effortItem ?? item, effortItem != null ? ChosenEffortType.Specified : ChosenEffortType.Default));
                }
            }

            return effortItemsModel;
        }

        private static EffortItemModel GetEffortItemModel(EffortItem item, ChosenEffortType type)
        {
            return new EffortItemModel
            {
                Id = item.Id,
                Name = item.Name,
                Choosed = false,
                HoursR_I = item.HoursR_I,
                HoursR_R = item.HoursR_R,
                EffortType = type
            };
        }
    }
}