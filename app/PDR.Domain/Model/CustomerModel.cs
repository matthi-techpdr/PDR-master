using PDR.Domain.Model.Enums;

namespace PDR.Domain.Model
{
    public class CustomerModel 
    {
        public virtual string Id { get; set; }

        public virtual string Name { get; set; } 

        public virtual StatesOfUSA? State { get; set; }

        public virtual string Phone { get; set; } 

        public virtual string City { get; set; }

        public virtual string Email { get; set; }

        public virtual string Email2 { get; set; }

        public virtual string Email3 { get; set; }

        public virtual string Email4 { get; set; }

        public virtual int AmountOfOpenEstimates { get; set; }

        public virtual double SumOfOpenEstimates { get; set; }

        public virtual double SumOfOpenWorkOrders { get; set; }

        public virtual double SumOfPaidInvoices { get; set; }

        public virtual double SumOfUnpaidInvoices { get; set; }
    }
}
