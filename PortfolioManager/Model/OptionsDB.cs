//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class OptionsDB
    {
        public long Id { get; set; }
        public string Issuer { get; set; }
        public string Symbol { get; set; }
        public string OptionType { get; set; }
        public bool IsTradable { get; set; }
        public double LastTradedPrice { get; set; }
        public Nullable<double> StrikePrice { get; set; }
        public System.DateTime MaturityDate { get; set; }
        public string ISIN { get; set; }
        public Nullable<double> Rebate { get; set; }
        public Nullable<double> Barrier { get; set; }
        public string BarrierOptionType { get; set; }
        public long UnderlyingID { get; set; }
        public long OptionKindID { get; set; }
    
        public virtual StockDB StockDB { get; set; }
        public virtual OptionKindDB OptionKindDB { get; set; }
    }
}