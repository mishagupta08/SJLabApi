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
    
    public partial class V_CurrentStockDetail_STK
    {
        public string PartyCode { get; set; }
        public string PartyName { get; set; }
        public decimal CatId { get; set; }
        public string CatName { get; set; }
        public string ProdId { get; set; }
        public string ProductName { get; set; }
        public string BatchCode { get; set; }
        public string Barcode { get; set; }
        public decimal MRP { get; set; }
        public decimal DP { get; set; }
        public decimal PurchaseRate { get; set; }
        public string ProdType { get; set; }
        public string StockType { get; set; }
        public decimal Qty { get; set; }
        public Nullable<decimal> StockValue { get; set; }
        public Nullable<decimal> DPStockValue { get; set; }
        public Nullable<decimal> MRPStockValue { get; set; }
    }
}