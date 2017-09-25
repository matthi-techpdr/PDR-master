namespace PDR.Domain.Model.Enums
{
    using System;
    [Flags]
    public enum UserRoles
    {
        Technician = 0,
        Manager = 1,
        Accountant = 2,
        Estimator = 4,
        Admin = 8,
        Superadmin = 16,
        Wholesaler = 32,
        Worker = 64,
        RITechnician = 128
    }
}
