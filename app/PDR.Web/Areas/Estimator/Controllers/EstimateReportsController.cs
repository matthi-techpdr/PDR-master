using PDR.Domain.Contracts.Repositories;
using PDR.Domain.Model;
using PDR.Domain.Model.Customers;
using PDR.Domain.Model.Reports;
using PDR.Domain.Model.Users;
using PDR.Domain.Services.Grid.Interfaces;
using PDR.Domain.Services.PDFConverters;
using PDR.Domain.Services.Webstorage;
using PDR.Web.Areas.Estimator.Models.Reports;
using PDR.Web.Core.Authorization;

namespace PDR.Web.Areas.Estimator.Controllers
{
    [PDRAuthorize(Roles = "Estimator")]
    public class EstimateReportsController : Common.Controllers.ReportsControllers.EstimateReportsController
    {
    }
}
