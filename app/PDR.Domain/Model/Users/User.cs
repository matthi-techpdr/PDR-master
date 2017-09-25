using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PDR.Domain.Model.Base;
using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model.Users
{
    public abstract class User : Entity
    {
        public virtual string Name { get; set; }

        public virtual string Login { get; set; }

        [JsonIgnore]
        public virtual string Password { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public abstract UserRoles Role
        {
            get; 
        }
    }
}