using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using PDR.Domain.Helpers;
using PDR.Domain.Model;
using PDR.Domain.Model.Enums;

namespace PDR.Web.Core.NLog.FileLoggers
{
    public static class EstimateLogger
    {
        private static readonly Logger Loggger = LogManager.GetCurrentClassLogger();

        private static string GetEstimateLog(Estimate estimate, string action)
        {
            var customer = estimate.Customer;
            var insurance = estimate.Insurance;
            var car = estimate.Car;
            var carInspections = estimate.CarInspections;
            var photosCount = estimate.Photos.Count();

            var customerInfo = new List<string>
                {
                    customer.CustomerType.ToString(),
                    customer.GetCustomerName(),
                    customer.Address1,
                    customer.Address2,
                    customer.City,
                    customer.State.ToString(),
                    customer.Zip,
                    customer.Phone,
                    customer.Fax,
                    customer.Email
                };
            

            var insuranceInfo = new List<string>
                {
                    insurance.InsuredName, insurance.CompanyName, insurance.Policy, insurance.Claim,
                    insurance.AccidentDate.HasValue ? insurance.AccidentDate.Value.ToString("MM/dd/yyyy") : null,
                    insurance.ClaimDate.HasValue ? insurance.ClaimDate.Value.ToString("MM/dd/yyyy") : null, insurance.Phone,
                    insurance.ContactName
                };

            var vehicleInfo = new List<string>
                {
                    car.VIN, car.Year.ToString(), car.Make, car.Model, car.Trim, car.Type.ToString(),
                    car.Mileage.ToString(), car.Color, car.LicensePlate, car.State.ToString(), car.CustRO, car.Stock
                };

            var carInspectionInfo = new List<string>();
            foreach (var insp in carInspections)
            {
                var str = new List<string>
                    {
                        insp.Name.ToString(),
                        insp.DentsAmount.GetTotalAmountName(),
                        insp.AverageSize.ToString(),
                        insp.Aluminium ? "Aluminium" : null,
                        insp.DoubleMetal ? "Double metal" : null
                    };

                str.AddRange(insp.ChosenEffortItems.Select(ef => string.Format("{0}, {1}, {2};", ef.EffortItem.Name, ef.Operations.ToString(), ef.Hours)));
                str.Add(insp.PriorDamage);
                str.AddRange(insp.CustomLines.Select(ef => string.Format("{0}, {1};", ef.Name, Math.Round(ef.Cost, 2))));
                str.Add(Math.Round(insp.PartsTotal, 2).ToString());
                carInspectionInfo.Add(GetSectionParams(str));
            }

            var estimatePriorDmg = estimate.PriorDamages;
            var customLines = estimate.CustomEstimateLines.Select(x => string.Join(";", x.Name + ", " + Math.Round(x.Cost, 2))).ToList();

            var template = string.Format("{0} estimate - {1}. {2}. Customer info: {3}. Insurance info: {4}. Vehicle info: {5}. Vehicle Repair Estimate - {6}: {7}. Photos: {8}. Prior damages: {9}. Custom lines: {10}",
            action,
            estimate.Id,
            estimate.EstimateStatus,
            GetSectionParams(customerInfo),
            insuranceInfo.Count() != 0 ? GetSectionParams(insuranceInfo) : "none",
            GetSectionParams(vehicleInfo),
            Math.Round(estimate.TotalAmount, 2),
            GetSectionParams(carInspectionInfo),
            photosCount,
            !string.IsNullOrWhiteSpace(estimatePriorDmg) ? estimatePriorDmg : "none",
            customLines.Count() != 0 ? GetSectionParams(customLines) : "none");

            return template.Replace("\n", string.Empty).Replace("\r", string.Empty);
        }

        private static string GetSectionParams(List<string> parameters)
        {
            parameters.RemoveAll(x => x == null);
            return string.Join("; ", parameters);
        }

        public static void Create(Estimate estimate)
        {
            try
            {
                var message = GetEstimateLog(estimate, "Create");
                Loggger.Info(message);
            }
            catch (Exception)
            {
            }
        }

        public static void Edit(Estimate estimate)
        {
            try
            {
                var message = GetEstimateLog(estimate, "Edit");
                Loggger.Info(message);
            }
            catch (Exception)
            {
            }
        }
    }
}