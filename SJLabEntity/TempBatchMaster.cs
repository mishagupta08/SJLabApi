//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SJLabEntity
{
    using System;
    using System.Collections.Generic;
    
    public partial class TempBatchMaster
    {
        public decimal ID { get; set; }
        public string BatchName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Timing { get; set; }
        public string ActiveStatus { get; set; }
        public string Trainer { get; set; }
        public decimal UserID { get; set; }
        public System.DateTime RecTimeStamp { get; set; }
        public string Action { get; set; }
        public Nullable<System.DateTime> ActionOn { get; set; }
        public Nullable<decimal> ByUser { get; set; }
    }
}
