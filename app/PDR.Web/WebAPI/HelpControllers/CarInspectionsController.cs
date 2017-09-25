using System;
using System.Linq;
using System.Web.Mvc;

using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Effort;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Web.Areas.Common.Models;
using PDR.Web.WebAPI.Authorization;
using PDR.Web.WebAPI.IphoneModels;

using SmartArch.Data;

namespace PDR.Web.WebAPI.HelpControllers
{
    [ApiAuthorize]
    public class CarInspectionsController : Controller
    {
        private readonly ICompanyRepository<Matrix> matrixRepository;

        private readonly ICompanyRepository<CarModel> carModelRepository;

        public CarInspectionsController(ICompanyRepository<Matrix> matrixRepository, ICompanyRepository<CarModel> carModelRepository)
        {
            this.matrixRepository = matrixRepository;
            this.carModelRepository = carModelRepository;
        }

        public JsonResult GetDentsCost(PriceModel model)
        {
            var matrix = this.matrixRepository.Get(model.MatrixId);
            if (matrix != null)
            {
                Func<MatrixPrice, bool> query =
                    x => x.AverageSize.Equals(model.AverageSize) && x.TotalDents.Equals(model.TotalDents) && x.PartOfBody.Equals(model.PartOfBody);

                var singleOrDefault = matrix.MatrixPrices.SingleOrDefault(query);
                if (singleOrDefault != null)
                {
                    var cost = singleOrDefault.Cost;
                    return this.Json(new { Cost = Math.Round(cost, 2) }, JsonRequestBehavior.AllowGet);
                }
            }

            return this.Json(new { Cost = 0.0 }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOversizedDentsCost(int count, long matrixId)
        {
            var matrix = this.matrixRepository.Get(matrixId);
            var cost = count * matrix.OversizedDents;
            return this.Json(new { Cost = Math.Round(cost, 2) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAdditionalPercents(AdditionalPersents model)
        {
            var matrix = this.matrixRepository.Get(model.MatrixId);
            var persents = 0;
            if (model.Aluminium)
            {
                persents = +matrix.AluminiumPanel;
            }

            if (model.DoubleMetal)
            {
                persents = +matrix.DoubleLayeredPanels;
            }

            if (model.OversizedRoof)
            {
                persents = +matrix.OversizedRoof;
            }

            model.Percents = persents;

            return this.Json(model, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCorrosionProtection(long matrixId)
        {
            var matrix = this.matrixRepository.Get(matrixId);
            return this.Json(new { Cost = matrix.CorrosionProtectionPart }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEfforts(string make, string model, int year, string part)
        {
            var defaultCar = this.carModelRepository.SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());
            var car = this.carModelRepository.SingleOrDefault(x => x.Make == make && x.Model == model && x.YearFrom <= year && x.YearTo >= year)
                ?? defaultCar;

            var bodyPart = (PartOfBody)Enum.Parse(typeof(PartOfBody), part, true);
            var efforts = EffortsDataModel.GetSectionEffortItemViewModels(car, defaultCar)
                .Where(x => x.Name == bodyPart)
                .Select(x => new
                {
                    x.Cost,
                    Name = x.Name.ToString(),
                    EffortItems = x.EffortItems.Select(
                        e => new
                                {
                                    e.Id,
                                    e.Name,
                                    EffortType = e.EffortType.ToString(),
                                    e.HoursR_I,
                                    e.HoursR_R
                                })
                 });
            
            return this.Json(new { Data = efforts }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetNewSectionPrice(string make, string model, int year)
        {
            var defaultCar = this.carModelRepository.SingleOrDefault(x => x.Make.ToLower() == "DefaultCar".ToLower());
            var car = this.carModelRepository.SingleOrDefault(x => x.Make == make && x.Model == model && x.YearFrom <= year && x.YearTo >= year)
                ?? defaultCar;

            var carParts = EffortsDataModel.GetSectionEffortItemViewModels(car, defaultCar)
                .Select(x => new
                {
                    NewSectionPrice = x.Cost,
                    Name = x.Name.ToString()
                    
                });
            return this.Json(carParts, JsonRequestBehavior.AllowGet);}
    }
}