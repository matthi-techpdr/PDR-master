using System.Collections.Generic;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class AddSuplementsModel
    {
        public AddSuplementsModel()
        {
            this.Supplements = new List<ApiSupplementModel>();
        }

        public long RepairOrderId { get; set; }

        public IEnumerable<ApiSupplementModel> Supplements { get; set; }
    }
}