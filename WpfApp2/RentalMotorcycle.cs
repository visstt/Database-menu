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
    
    public partial class RentalMotorcycle
    {
        public int RentalMotorcycleID { get; set; }
        public int RentalID { get; set; }
        public int MotorcycleID { get; set; }
    
        public virtual Motorcycle Motorcycle { get; set; }
        public virtual Rental Rental { get; set; }
    }
}
