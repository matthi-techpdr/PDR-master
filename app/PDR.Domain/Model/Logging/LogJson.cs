using System;
using System.IO;
using System.Web.Script.Serialization;

namespace PDR.Domain.Model.Logging
{
    public class LogJson
    {
        private readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        public void Log(object obj)
        {
            var json = new JavaScriptSerializer().Serialize(obj);
            var filename = obj.GetType().Name + "_" + DateTime.Now.ToFileTime() + ".txt";
            using (var file = File.CreateText(Path.Combine(this.path, filename)))
            {
                file.Write(json);
            }
        }
    }
}
