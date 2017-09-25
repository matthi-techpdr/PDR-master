using PDR.Domain.Model;
using PDR.Domain.Services.Grid.Interfaces;

namespace PDR.Web.Areas.SuperAdmin.Models
{
    public class IphoneAppJsonModel : IJsonModel
    {
        public IphoneAppJsonModel()
        {
        }

        public IphoneAppJsonModel(VersioniPhoneApp versioniPhoneApp)
        {
            this.Id = versioniPhoneApp.Id.ToString();
            this.DateUpload = versioniPhoneApp.DateUpload.ToString("MM/dd/yyyy");
            this.IsWorkingBild = versioniPhoneApp.IsWorkingBild ? "Working" : " End of Life";
            this.IsDownLoaded = versioniPhoneApp.IsDownLoaded ? "Yes" : "No";
            this.LocalStoragePath = versioniPhoneApp.LocalStoragePath;
            this.Version = versioniPhoneApp.Version;
        }

        public string Id { get; protected set; }

        public string Version { get; set; }

        public string LocalStoragePath { get; set; }
        
        public string IsWorkingBild { get; set; }

        public string IsDownLoaded { get; set; }

        public string DateUpload { get; set; }
    }
}