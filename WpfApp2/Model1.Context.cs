﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Nochnie_Volki_OrenburgEntities : DbContext
    {
        public Nochnie_Volki_OrenburgEntities()
            : base("name=Nochnie_Volki_OrenburgEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<DiscountSystem> DiscountSystem { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeePosition> EmployeePosition { get; set; }
        public virtual DbSet<EmployeeStatus> EmployeeStatus { get; set; }
        public virtual DbSet<Motorcycle> Motorcycle { get; set; }
        public virtual DbSet<MotorcycleBrand> MotorcycleBrand { get; set; }
        public virtual DbSet<MotorcycleType> MotorcycleType { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PenaltySystem> PenaltySystem { get; set; }
        public virtual DbSet<Rental> Rental { get; set; }
        public virtual DbSet<RentalMotorcycle> RentalMotorcycle { get; set; }
        public virtual DbSet<RentalStatus> RentalStatus { get; set; }
    }
}