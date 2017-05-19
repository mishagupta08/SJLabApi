using System;
using System.Collections.Generic;
using System.Linq;
using SJLInvEntity;
using SJLabEntity;
using SJLABSAPI.Models;
using Newtonsoft.Json;

namespace SJLABSAPI.Service
{
    public class ProductService
    {
        public string GetProductList()
        {
            string response = string.Empty;
            List<ProductNameList> productList = new List<ProductNameList>();
            try {
                using (var db = new SJLInvEntities())
                {
                    productList = (from r in db.M_ProductMaster where r.ActiveStatus == "Y" && r.OnWebSite == "Y" select new ProductNameList
                    {
                        id = r.ProdId,
                        name = r.ProductName,
                        MRP = r.MRP,
                        BV = r.BV,
                        DP = r.DP,
                        RP = r.RP
                    }).OrderBy(o => o.name).ToList();
                    response = "{\"products\":" + JsonConvert.SerializeObject(productList) + ",\"response\":\"OK\"}";
                }
            }
            catch (Exception ex) {
                response = "{\"response\":\"FAILED\"}";
            }
            return response;
        }

        public string getbalance(string formno)
        {
            string response = string.Empty;
            List<ProductNameList> productList = new List<ProductNameList>();
            try
            {
                using (var db = new SjLabsEntities())
                {
                     
                }
            }
            catch (Exception ex)
            {
                response = "{\"response\":\"FAILED\"}";
            }
            return response;
        }
    }
}
