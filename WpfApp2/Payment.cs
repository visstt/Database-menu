//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApp2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payment
    {
        public int PaymentID { get; set; }
        public int RentalID { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public int CustomerID { get; set; }
        public string PaymentStatus { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Rental Rental { get; set; }
    }
}
