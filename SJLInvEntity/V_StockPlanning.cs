//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SJLInvEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class V_StockPlanning
    {
        public decimal GroupId { get; set; }
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public decimal CatId { get; set; }
        public string ProdID { get; set; }
        public string ProductName { get; set; }
        public decimal PurRate { get; set; }
        public decimal MRP { get; set; }
        public decimal DP { get; set; }
        public Nullable<decimal> Qty { get; set; }
        public Nullable<decimal> AvailQty { get; set; }
        public Nullable<decimal> PurValue { get; set; }
        public Nullable<decimal> MRPValue { get; set; }
        public Nullable<decimal> DPValue { get; set; }
        public Nullable<decimal> StockHold { get; set; }
        public Nullable<decimal> StockHoldValue { get; set; }
    }
}