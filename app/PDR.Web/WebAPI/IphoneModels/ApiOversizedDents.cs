using System;

using PDR.Domain.Model.CustomLines;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ApiOversizedDents
    {
        public ApiOversizedDents()
        {
        }

        public ApiOversizedDents(CustomLine line)
        {
            if (line != null)
            {
                this.Id = line.Id;
                this.Count = Convert.ToDouble(line.Name);
                this.Cost = line.Cost;
            }
        }

        public long Id { get; set; }

        public double Count { get; set; }

        public double Cost { get; set; }
    }
}