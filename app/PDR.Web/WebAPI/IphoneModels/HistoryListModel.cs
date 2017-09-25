using System.Collections.Generic;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class HistoryListModel:List<HistoryModel>
    {
        public HistoryListModel(List<string> history)
        {
            foreach (var h in history)
            {
                this.Add(new HistoryModel(h));
            }
        }
    }
}