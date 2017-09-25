using System.Collections.Generic;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class CompanyListModel:List<CompanyModel>
    {
        public CompanyListModel(List<string> companyNames)
        {
            foreach (var companyName in companyNames)
            {
                this.Add(new CompanyModel(companyName));
            }
        }
    }
}