using System;
using PDR.Domain.Model.Base;

namespace PDR.Domain.Model.Users
{
    public class FormerRI : Entity, ICompanyEntity
    {
        public virtual Employee Employee { get; set; }

        public virtual Company Company { get; set; }

        public virtual DateTime RoleChangeDate { get; set; }
    }
}