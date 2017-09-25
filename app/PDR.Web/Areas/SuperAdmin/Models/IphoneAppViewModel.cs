using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.SuperAdmin.Models
{
    public class IphoneAppViewModel : IViewModel
    {
        public string Id { get; set; }
        
        public string Version { get; set; }

        public string LocalStoragePath { get; set; }

        public string UrlToDownload { get; set; }

        public bool IsWorkingBild { get; set; }

        public bool IsDownLoaded { get; set; }

        public string DateUpload { get; set; }
    }
}