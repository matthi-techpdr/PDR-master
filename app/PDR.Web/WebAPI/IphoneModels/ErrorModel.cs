using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Elmah;

namespace PDR.Web.WebAPI.IphoneModels
{
    public class ErrorModel
    {
        public ErrorModel(string message)
        {
            this.Message = message;
        }
        public string Message { get; set; }
    }
}