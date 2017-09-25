using System.Collections.Generic;

using PDR.Domain.Helpers;
using PDR.Domain.Model.Matrixes;

namespace PDR.Domain.Services.XLS
{
    public class OrderHelper
    {
        public MatrixPrice DIME { get; set; }

        public MatrixPrice NKL { get; set; }

        public MatrixPrice QTR { get; set; }

        public MatrixPrice HALF { get; set; }

        public List<string> ToOrderedArray()
        {
            var list = new List<string>
                {
                    this.ToCostConverter(this.DIME),
                    this.ToCostConverter(this.NKL),
                    this.ToCostConverter(this.QTR),
                    this.ToCostConverter(this.HALF)
                };

            return list;
        }

        public Dictionary<long, double> ToOrderedModelPairs()
        {
            var list = new Dictionary<long, double>();
            list[this.DIME.Id == 0 ? 1 : this.DIME.Id] = this.DIME.Cost;
            list[this.NKL.Id == 0 ? 2 : this.NKL.Id] = this.NKL.Cost;
            list[this.QTR.Id == 0 ? 3 : this.QTR.Id] = this.QTR.Cost;
            list[this.HALF.Id == 0 ? 4 : this.HALF.Id] = this.HALF.Cost;
            return list;
        }

        private string ToCostConverter(MatrixPrice matrixPrice)
        {
            return matrixPrice == null ? string.Empty : matrixPrice.Cost.ToString();
        }
    }
}
