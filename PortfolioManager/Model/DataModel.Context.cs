﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PortfolioManager.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DataModelContainer : DbContext
    {
        public DataModelContainer()
            : base("name=DataModelContainer")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SecurityTypeDB> SecurityTypeDBs { get; set; }
        public virtual DbSet<InstrumentsDB> InstrumentsDBs { get; set; }
        public virtual DbSet<OrderBookDB> OrderBookDBs { get; set; }
        public virtual DbSet<OptionKindDB> OptionKindDBs { get; set; }
        public virtual DbSet<OptionsDB> OptionsDBs { get; set; }
        public virtual DbSet<StockDB> StockDBs { get; set; }
        public virtual DbSet<InterestRateDB> InterestRateDBs { get; set; }
    }
}
