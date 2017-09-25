using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Matrixes;

namespace PDR.Web.Areas.Estimator.Models.Estimates
{
    public class EstimateWholesaleCustomerModel
    {
        public EstimateWholesaleCustomerModel()
        {
            this.Customers = new List<SelectListItem>();
            this.Matrices = new List<SelectListItem>();
        }
        
        public EstimateWholesaleCustomerModel(IEnumerable<WholesaleCustomer> customers, IEnumerable<Matrix> matrices)
        {
            this.InitCustomers(customers);
            this.InitMatrices(matrices);
        }

        public void InitCustomers(IEnumerable<WholesaleCustomer> customers)
        {
            this.Customers = customers.OrderBy(m => m.Name).Select(x => new SelectListItem
                                                                            {
                                                                                Value = x.Id.ToString(),
                                                                                Text = x.Name.Length > 37
                                                                                        ? x.Name.Substring(0, 37) + "..."
                                                                                        : x.Name
                                                                            }).ToList();
            this.Customers.Insert(0, new SelectListItem{Value = "0", Text = string.Empty});
        }

        public void InitMatrices(IEnumerable<Matrix> matrices)
        {
            this.Matrices = matrices.OrderBy(x => x.Name).Select(x => new SelectListItem
                                                                          {
                                                                              Value = x.Id.ToString(),
                                                                              Text = x.Name
                                                                          }).ToList();
            this.Matrices.Insert(0, new SelectListItem { Value = "0", Text = string.Empty });
        }

        public long CustomerId { get; set; }

        public long MatrixId { get; set; }

        public IList<SelectListItem> Customers { get; set; }

        public IList<SelectListItem> Matrices { get; set; }
    }
}