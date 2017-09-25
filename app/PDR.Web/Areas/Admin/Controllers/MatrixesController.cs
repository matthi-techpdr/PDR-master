using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using MvcContrib.Pagination;
using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model.Enums;
using PDR.Domain.Model.Matrixes;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Webstorage;
using PDR.Domain.Services.XLS;
using PDR.Web.Areas.Admin.Models.Matrix;
using PDR.Web.Core.Authorization;
using PDR.Web.Core.NLog.FileLoggers;

using SmartArch.Data;
using SmartArch.Web.Attributes;

namespace PDR.Web.Areas.Admin.Controllers
{
    [PDRAuthorize(Roles = "Admin")]
    public class MatrixesController : Controller
    {
        private readonly ICompanyRepository<Matrix> matrixRepository;

        private readonly IXLSGenerator xlsGenerator;

        private readonly Employee currentEmployee;

        public MatrixesController(
            ICompanyRepository<Matrix> matrixRepository,
            IXLSGenerator xlsGenerator,
            ICurrentWebStorage<Employee> storage)
        {
            this.matrixRepository = matrixRepository;
            this.xlsGenerator = xlsGenerator;
            this.currentEmployee = storage.Get();
        }

        public ActionResult Index(int? page)
        {
            IPagination<MatrixListModel> matrixList = 
                this.matrixRepository
                .OrderBy(x => x.Name)
                .Select(x => new MatrixListModel
                {
                    Title = x.Name,
                    Description = x.Description,
                    Id = x.Id.ToString(),
                    StatusAction = x.Status == Statuses.Active ? "Suspend" : "Activate"
                })
                .AsPagination(page ?? 1, 5);

            return this.View(matrixList);
        }

        public ActionResult RenderMatrix(long? id)
        {
            var defaultMatrix = this.currentEmployee.Company.DefaultMatrix;
            Matrix matrix = id.HasValue ? this.matrixRepository.Get(id.Value) : defaultMatrix;

            var model = new MatrixModel(matrix);
            if (!id.HasValue)
            {
                model.Id = null;
                model.Name = "New";
                model.Description = null;
            }

            return this.View(model);
        }

        public bool NameIsUnique(string name, string matrixId)
        {
            var matrix = this.matrixRepository.SingleOrDefault(x => x.Name == name);
            if (matrix == null)
            {
                return true;
            }

            var id = matrixId == "null" ? null : matrixId;
            if (id != null)
            {
                return matrix.Id.ToString() == id;
            }

            return false;
        }

        [Transaction]
        public void Save(MatrixModel model)
        {
            if (ModelState.IsValid)
            {
                var matrix = model.Id != null
                                 ? this.matrixRepository.Get(Convert.ToInt64(model.Id))
                                 : new PriceMatrix(true);
                matrix.Name = model.Name;
                matrix.AluminiumPanel = int.Parse(model.AluminumPanel);
                matrix.CorrosionProtectionPart = int.Parse(model.PerBodyPart);
                matrix.OversizedRoof = int.Parse(model.Suv);
                matrix.Maximum = int.Parse(model.Max);
                matrix.DoubleLayeredPanels = int.Parse(model.DoubleLayeredPanels);
                matrix.MaxCorrosionProtection = double.Parse(model.PerWholeCar);
                matrix.Description = model.Description;
                matrix.OversizedDents = double.Parse(model.OversizedDents);
                

                if (model.Id != null)
                {
                    foreach (var price in model.MatrixPrices)
                    {
                        var mPrice = matrix.MatrixPrices.SingleOrDefault(x => x.Id == price.Id);
                        if (mPrice != null)
                        {
                            mPrice.Cost = price.Cost;
                        }
                    }
                }
                else
                {
                    var defaultMatrix = this.currentEmployee.Company.DefaultMatrix;
                    foreach (var price in model.MatrixPrices)
                    {
                        var defPrice = defaultMatrix.MatrixPrices.SingleOrDefault(x => x.Id == price.Id);
                        if (defPrice != null)
                        {
                            var newPrice = new MatrixPrice(true)
                                {
                                    AverageSize = defPrice.AverageSize,
                                    PartOfBody = defPrice.PartOfBody,
                                    TotalDents = defPrice.TotalDents,
                                    Cost = price.Cost
                                };
                            matrix.AddMatrixPrice(newPrice);
                        }
                    }
                }

                if (model.Id != null)
                {
                    MatrixLogger.Edit(matrix);
                }
                else
                {
                    MatrixLogger.Create(matrix);
                }

                this.matrixRepository.Save(matrix);
            }
        }

        public FileContentResult ExportToXLS(long id)
        {
            var matrix = this.matrixRepository.Get(id);
            var xls = this.xlsGenerator.GenerateXLS(matrix);
            return new FileContentResult(xls, "application/vnd.ms-excel")
                {
                    FileDownloadName = string.Format("{0}({1}).xls", matrix.Name, DateTime.Now.ToString()) 
                };
        }

        [Transaction]
        [HttpPost]
        public ContentResult ChangeStatus(long id)
        {
            var matrix = this.matrixRepository.Get(id);
            string label;
            if (matrix.Status == Statuses.Active)
            {
                matrix.Status = Statuses.Suspended;
                MatrixLogger.Suspend(matrix);
                label = "Activate";
                var priceMatrix = matrix as PriceMatrix;
                if (priceMatrix != null)
                {
                    foreach (var customer in priceMatrix.Customers)
                    {
                        customer.Matrices.Remove(priceMatrix);
                    }
                }
            }
            else
            {
                matrix.Status = Statuses.Active;
                MatrixLogger.Reactivate(matrix);
                label = "Suspend";
            }

            this.matrixRepository.Save(matrix);
            return this.Content(label);
        }

        [Transaction]
        public ActionResult Duplicate(long id)
        {
            var matrix = this.matrixRepository.Get(id);
            var duplicatedMatrix = new PriceMatrix(true)
                {
                    Name = string.Format("{0} - Copy", matrix.Name.Length > 18 ? matrix.Name.Substring(0, 18) : matrix.Name),
                    Description = matrix.Description,
                    AluminiumPanel = matrix.AluminiumPanel, 
                    DoubleLayeredPanels = matrix.DoubleLayeredPanels, 
                    OversizedRoof = matrix.OversizedRoof, 
                    Maximum = matrix.Maximum, 
                    CorrosionProtectionPart = matrix.CorrosionProtectionPart, 
                    MaxCorrosionProtection = matrix.MaxCorrosionProtection, 
                    OversizedDents = matrix.OversizedDents,
                    Status = matrix.Status
                };

            foreach (var prototypePrice in matrix.MatrixPrices)
            {
                var price = new MatrixPrice(true)
                    {
                        AverageSize = prototypePrice.AverageSize, 
                        Cost = prototypePrice.Cost, 
                        PartOfBody = prototypePrice.PartOfBody, 
                        TotalDents = prototypePrice.TotalDents
                    };
                duplicatedMatrix.AddMatrixPrice(price);
            }

            if (matrix is PriceMatrix)
            {
                foreach (var prototypeCustomer in (matrix as PriceMatrix).Customers)
                {
                    (matrix as PriceMatrix).AddCustomer(prototypeCustomer);
                }
            }

            MatrixLogger.Duplicate(matrix);
            this.matrixRepository.Save(duplicatedMatrix);
            return this.RedirectToAction("Index", new { page = 1 });
        }

        [Transaction]
        public ActionResult ImportFromXls()
        {
            var files = HttpContext.Request.Files;
            var file = files[0];
            if (file != null)
            {
                var id = HttpContext.Request.Headers["id"];
                var name = HttpContext.Request.Headers["name"];
                var description = HttpContext.Request.Headers["description"];
                var memoryStream = new MemoryStream();
                file.InputStream.CopyTo(memoryStream);
                try
                {
                    long mId;
                    var currentMatrix = long.TryParse(id, out mId) ? this.matrixRepository.Get(mId) : this.currentEmployee.Company.DefaultMatrix;
                    var currentMatrixModel = new MatrixModel(currentMatrix);
                    var matrixModel = new MatrixModel(this.xlsGenerator.ImportMatrixFromXLS(memoryStream, new PriceMatrix(true)));
                    
                    for (var i = 0; i < matrixModel.Columns.Count(); i++)
                    {
                        var column = matrixModel.Columns.ElementAt(i);
                        for (var j = 0; j < column.Prices.Count(); j++)
                        {
                            var prices = matrixModel.Columns.ElementAt(i).Prices.ElementAt(j);
                            for (var z = 0; z < prices.Values.Count(); z++)
                            {
                                var key = currentMatrixModel.Columns.ElementAt(i).Prices.ElementAt(j).ElementAt(z).Key;
                                currentMatrixModel.Columns.ElementAt(i).Prices.ElementAt(j)[key] = matrixModel.Columns.ElementAt(i).Prices.ElementAt(j).Values.ElementAt(z);
                            }
                        }
                    }

                    currentMatrixModel.AluminumPanel = matrixModel.AluminumPanel;
                    currentMatrixModel.DoubleLayeredPanels = matrixModel.DoubleLayeredPanels;
                    currentMatrixModel.Suv = matrixModel.Suv;
                    currentMatrixModel.Max = matrixModel.Max;
                    currentMatrixModel.PerBodyPart = matrixModel.PerBodyPart;
                    currentMatrixModel.PerWholeCar = matrixModel.PerWholeCar;
                    currentMatrixModel.OversizedDents = matrixModel.OversizedDents;
                    currentMatrixModel.Name = name;
                    currentMatrixModel.Description = description;

                    if (mId == 0)
                    {
                        currentMatrixModel.Id = null;
                    }

                    return this.PartialView("RenderMatrix", currentMatrixModel);
                }
                catch (Exception)
                {
                    return this.Content("<p id=\"matrixError\">Incorrect matrix format. Values in matrix price table must to be numbers.</p>");
                }
            }

            return this.Content("XLS file error.");
        }
    }
}
