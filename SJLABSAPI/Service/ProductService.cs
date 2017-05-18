using System;
using System.Collections.Generic;
using System.Linq;
using SJLInvEntity;
using SJLABSAPI.Models;

namespace SJLABSAPI.Service
{
    public class ProductService
    {
        public List<ProductNameList> GetProductList()
        {
            List<ProductNameList> productList = new List<ProductNameList>();
            try { 
                using(var db = new SJLInvEntities())
                {
                    productList = (from r in db.M_ProductMaster where r.ActiveStatus=="Y" && r.OnWebSite=="Y" select new ProductNameList
                                   {
                                     id = r.ProdId,
                                     name = r.ProductName,
                                     MRP=r.MRP,
                                     BV=r.BV,
                                     DP=r.DP,
                                     RP=r.RP
                                   }).OrderBy(o=>o.name).ToList();
                }
            }
            catch (Exception ex){
                throw ex;
            }
            return productList;
        }
    }
}
