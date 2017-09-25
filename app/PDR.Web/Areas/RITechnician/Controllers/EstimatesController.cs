using System;
using System.Web.Mvc;
using System.Linq;

using Microsoft.Practices.ServiceLocation;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Effort;
using PDR.Domain.Services.Logging;

using PDR.Web.Areas.Common.Models;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.RITechnician.Controllers
{
    [PDRAuthorize(Roles = "RITechnician")]
    public class EstimatesController: Controller 
    {
        private readonly IRepositoryFactory repositoryFactory;
        private readonly ILogger logger;

        public EstimatesController(IRepositoryFactory repositoryFactory, ILogger logger)
        {
            this.repositoryFactory = repositoryFactory;
            this.logger = logger;
        }

        [HttpPost]
        public JsonResult GetEffortHours(int year, string model, string make, string id = null)
        {
            var carRepo = ServiceLocator.Current.GetInstance<ICompanyRepository<CarModel>>();
            var cars = ServiceLocator.Current.GetInstance<ICompanyRepository<Car>>();
            Car currentcar = null;
            if (id != null)
            {
                currentcar = cars.SingleOrDefault(x => x.Id == Convert.ToInt64(id));
            }
            var defaultCar = this.repositoryFactory
                                    .CreateForCompany<CarModel>()
                                    .SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());
            var car = carRepo.SingleOrDefault(x => x.Make.ToLower() == make.ToLower()
                                                && x.Model.ToLower() == model.ToLower()
                                                && x.YearFrom <= year
                                                && x.YearTo >= year)
                                                ?? defaultCar;

            return Json(
                car != null ? new
                {
                    Data = EffortsDataModel.GetSectionEffortItemViewModels(car, defaultCar),
                    CarMake = car.Make,
                    CarType = currentcar != null ? currentcar.Type.ToString() : car.Type.ToString()
                }
                : null,
                JsonRequestBehavior.AllowGet);
        }
    }
}