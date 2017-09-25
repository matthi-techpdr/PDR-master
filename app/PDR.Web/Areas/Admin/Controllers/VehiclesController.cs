using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Effort;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Web.Areas.Admin.Models.Vehicle;
using PDR.Web.Core.Authorization;

using SmartArch.Data;
using SmartArch.Web.Attributes;
using IRepositoryFactory = PDR.Domain.Contracts.Repositories.IRepositoryFactory;

namespace PDR.Web.Areas.Admin.Controllers
{
    [PDRAuthorize(Roles = "Admin")]
    public class VehiclesController : Controller
    {
        private readonly IRepositoryFactory repositoryFactory;

        private readonly ICompanyRepository<CarModel> carsRepository;

        private readonly IGridMaster<CarModel, VehicleJsonModel, VehicleViewModel> vehicleGridMaster;

        public VehiclesController(
            IRepositoryFactory repositoryFactory,
            ICompanyRepository<CarModel> carsRepository,
            IGridMaster<CarModel, VehicleJsonModel, VehicleViewModel> vehicleGridMaster)
        {
            this.repositoryFactory = repositoryFactory;
            this.carsRepository = carsRepository;
            this.vehicleGridMaster = vehicleGridMaster;
        }

        public ActionResult Index()
        {
            var makes =
                this.carsRepository.Where(x => x.Make.ToLower() != "DefaultCar".ToLower()).OrderBy(x => x.Make).Select(x => x.Make).ToList().Distinct()
                .Select(x => new SelectListItem { Text = x, Value = x }).ToList();
            makes.Insert(0, new SelectListItem { Text = @"All vehicles", Value = string.Empty });
            makes.Insert(1, new SelectListItem { Text = @"Default", Value = @"DefaultCar" });
            ViewData["makes"] = makes;

            return View();
        }

        public JsonResult GetData(string sidx, string sord, int page, int rows, string make)
        {
            if (!string.IsNullOrWhiteSpace(make))
            {
                this.vehicleGridMaster.ExpressionFilters.Add(x => x.Make.ToLower() == make.ToLower());
            }

            var data = this.vehicleGridMaster.GetData(page, rows, sidx, sord);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region New

        [HttpGet]
        public ActionResult New()
        {
            var carModel = this.carsRepository.SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());

            #region SerializeToXML

            ////var model = new VehicleDefaultModel();

            ////model.Make = carModel.Make;
            ////model.Model = carModel.Model;
            ////model.YearFrom = 1756;
            ////model.YearTo = 1756;
            ////foreach (var part in carModel.CarParts)
            ////{
            ////    var section = new VehicleDefaultSectionModel
            ////                           {
            ////                               Name = part.Name,
            ////                               NewSectionPrice = part.NewSectionPrice
            ////                           };

            ////    foreach (var item in part.EffortItems)
            ////    {
            ////        section.EffortItems.Add(new VehicleDefaultEffortItemModel
            ////                                    {
            ////                                        HoursR_I = item.HoursR_I,
            ////                                        HoursR_R = item.HoursR_R,
            ////                                        Name = item.Name
            ////                                    });
            ////    }

            ////    model.CarParts.Add(section);
            ////}
            ////this.SerializeToXML(model);

            #endregion

            var vehicle = new VehicleModel(carModel, "New");

            return this.View(vehicle);
        }

        [HttpPost]
        [Transaction]
        public ActionResult New(VehicleModel vehicleModel)
        {
            if (ModelState.IsValid)
            {
                var carModel = new CarModel(true);
                this.Save(carModel, vehicleModel, true);

                return this.RedirectToAction("Index");
            }

            return this.View(vehicleModel);
        }

        #endregion

        #region Edit

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var vehicle = this.carsRepository.Get(id);
            return this.View(new VehicleModel(vehicle, "Edit"));
        }

        [HttpPost]
        [Transaction]
        public ActionResult Edit(VehicleModel vehicleModel)
        {
            if (ModelState.IsValid)
            {
                var vehicle = this.carsRepository.Get(Convert.ToInt64(vehicleModel.Info.Id));
                if (vehicle == null)
                {
                    throw new HttpException(404, "Not found action");
                }

                this.Save(vehicle, vehicleModel);

                return this.RedirectToAction("Index");
            }

            return this.View(vehicleModel);
        }

        #endregion

        #region View

        [HttpGet]
        [Transaction]
        public ActionResult View(long? id)
        {
            var vehicle = this.carsRepository.Get(Convert.ToInt64(id));
            var model = new VehicleModel(vehicle, "View");
            return this.View(model);
        }

        #endregion

        public ActionResult Duplicate(long id)
        {
            var vehicle = this.carsRepository.Get(id);

            return this.View(new VehicleModel(vehicle, "Duplicate"));
        }

        public JsonResult UniqueVehicle(int from, int to, string make, string model, long? id)
        {
            CarModel vehicle;
            if (id.HasValue)
            {
                vehicle = this.carsRepository.Get(id.Value);
                if (vehicle.Make.ToLower() == make.ToLower()
                    && vehicle.Model.ToLower() == model.ToLower()
                    && vehicle.YearFrom == from
                    && vehicle.YearTo == to)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }

            vehicle = this.carsRepository.SingleOrDefault(x => x.Make.ToLower() == make
                                                                       && x.Model.ToLower() == model
                                                                       && x.YearFrom == from
                                                                       && x.YearTo == to);


            return Json(vehicle != null, JsonRequestBehavior.AllowGet);
        }

        #region Private Methods

        ////public void SerializeToXML(VehicleDefaultModel model)
        ////{
        ////    var serializer = new XmlSerializer(typeof(VehicleDefaultModel));
        ////    Stream stream = new FileStream(this.Server.MapPath(@"~\Content\DefaultVehicle\default_vehicle.xml"), FileMode.Create);
        ////    serializer.Serialize(stream, model);
        ////    stream.Close();
        ////}

        private void Save(CarModel carModel, VehicleModel vehicleModel, bool isNew = false)
        {
            var resolver = new VehicleSaveResolver(this.repositoryFactory, isNew);
            resolver.Save(carModel, vehicleModel);

            this.carsRepository.Save(carModel);
        }

        #endregion
    }
}
