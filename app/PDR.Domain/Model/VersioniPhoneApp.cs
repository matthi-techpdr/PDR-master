using System;

using PDR.Domain.Model.Base;

namespace PDR.Domain.Model
{
    public class VersioniPhoneApp : Entity
    {
        public virtual string Version { get; set; }

        public virtual string LocalStoragePath { get; set; }
        
        public virtual bool IsWorkingBild { get; set; }

        public virtual bool IsDownLoaded { get; set; }

        public virtual DateTime DateUpload { get; set; }
    }
}