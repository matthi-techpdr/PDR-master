using System;
using System.Web;
using System.Web.Caching;

namespace PDR.Domain.Services.Sheduler
{
    public class Sheduler : ISheduler
    {
        private Action WhatToDo { get; set; }

        public void RunProcessByTime(DateTime timeToRun, Action whatToDo)
        {
            this.WhatToDo = whatToDo;
            var onRemove = new CacheItemRemovedCallback(this.RemovedCallback);
            HttpContext.Current.Cache.Add(Guid.NewGuid().ToString(), string.Empty, null, timeToRun, Cache.NoSlidingExpiration, CacheItemPriority.High, onRemove);
        }

        private void RemovedCallback(string key, object value, CacheItemRemovedReason removedReason)
        {
            this.WhatToDo.Invoke();
        }
    }
}
