using System.Collections.Generic;

namespace PDR.Web.Areas.Admin.Models.Matrix
{
    public class ColumnViewModel
    {
        public int Dent { get; set; }

        public IEnumerable<Dictionary<long, double>> Prices { get; set; }
    }
}