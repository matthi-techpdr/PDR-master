using System;
using System.Collections.Generic;
using System.Linq;
using PDR.Domain.Model.Enums;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.XLS;

namespace PDR.Web.Areas.Admin.Models.Matrix
{
    public class MatrixModel : IViewModel
    {
        private static TotalDents[] exceptTotalDentses = {TotalDents.NoDamage, TotalDents.Conventional};

        public MatrixModel()
        {
        }



        public MatrixModel(Domain.Model.Matrixes.Matrix matrix)
        {
            this.Id = matrix.Id.ToString();
            this.FillColumns(matrix);
            this.AluminumPanel = matrix.AluminiumPanel.ToString();
            this.DoubleLayeredPanels = matrix.DoubleLayeredPanels.ToString();
            this.PerBodyPart = matrix.CorrosionProtectionPart.ToString();
            this.PerWholeCar = matrix.MaxCorrosionProtection.ToString();
            this.Max = matrix.Maximum.ToString();
            this.Suv = matrix.OversizedRoof.ToString();
            this.Name = matrix.Name;
            this.Description = matrix.Description;
            this.OversizedDents = matrix.OversizedDents.ToString();
        }

        public IEnumerable<ColumnViewModel> Columns { get; set; }

        public IEnumerable<PriceModel> MatrixPrices { get; set; } 

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string AluminumPanel { get; set; }

        public string DoubleLayeredPanels { get; set; }

        public string Suv { get; set; }

        public string Max { get; set; }

        public string OversizedDents { get; set; }

        public string PerBodyPart { get; set; }

        public string PerWholeCar { get; set; }

        private void FillColumns(Domain.Model.Matrixes.Matrix matrix)
        {
            IList<ColumnViewModel> model = new List<ColumnViewModel>();

            var allDents = Enum.GetValues(typeof(TotalDents)).Cast<TotalDents>().Except(exceptTotalDentses);
            foreach (var dent in allDents)
            {

                   var matrixprices = matrix.MatrixPrices
                        .Where(x => x.TotalDents == dent
                                    && x.PartOfBody != PartOfBody.FrontBumper
                                    && x.PartOfBody != PartOfBody.RearBumper);
                   var prices = matrixprices
                       //.GroupBy(x => x.PartOfBody)
                       .OrderBy(x => x.PartOfBody).ThenBy(x => x.TotalDents).ThenBy(x => x.AverageSize)
                       .GroupBy(x => x.PartOfBody)
                       .Select(
                           m =>
                           {
                               var orderModel = new OrderHelper();
                               orderModel.DIME = m.FirstOrDefault(s => s.AverageSize == AverageSize.DIME);
                               orderModel.NKL = m.FirstOrDefault(s => s.AverageSize == AverageSize.NKL);
                               orderModel.QTR = m.FirstOrDefault(s => s.AverageSize == AverageSize.QTR);
                               orderModel.HALF = m.FirstOrDefault(s => s.AverageSize == AverageSize.HALF);
                               return orderModel.ToOrderedModelPairs();
                           }).ToList();
                
                var column = new ColumnViewModel {Dent = (int) dent, Prices = prices};
                model.Add(column);
            }

            this.Columns = model;
        }
    }
}