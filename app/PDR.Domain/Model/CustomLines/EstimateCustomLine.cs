
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDR.Domain.Model.CustomLines
{
    using Microsoft.Practices.ServiceLocation;

    using PDR.Domain.Model.Users;
    using PDR.Domain.Services.Webstorage;

    /// <summary>
	/// Estimate custom line
	/// </summary>
	public class EstimateCustomLine : CustomLine
	{
		public virtual Estimate Estimate { get; set; }

        public EstimateCustomLine()
        {
            
        }

        public EstimateCustomLine(bool isNewEntity = false)
	    {
            if (isNewEntity)
            {
                var user = ServiceLocator.Current.GetInstance<ICurrentWebStorage<Employee>>().Get();
                this.Company = user.Company;
            }
        }
	}
}
