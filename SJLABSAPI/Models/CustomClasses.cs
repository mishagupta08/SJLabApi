using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SJLABSAPI.Models
{
    public class ProductNameList
    {
        public string id { get; set; }
        public string name { get; set; }
        public decimal MRP { get; set; }
        public decimal DP { get; set; }
        public decimal RP { get; set; }
        public decimal BV { get; set; }
    }

    public class DeliveryAddressList
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}