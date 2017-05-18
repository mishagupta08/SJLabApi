using System;
using System.Collections.Generic;
using System.Linq;
using SJLInvEntity;
using SJLABSAPI.Models;

namespace SJLABSAPI.Service
{
    public class LedgerService
    {
        public List<DeliveryAddressList> GetDeliveryAddressList()
        {
            List<DeliveryAddressList> AddressList = new List<DeliveryAddressList>();
            try { 
                using(var db = new SJLInvEntities())
                {
                    AddressList = (from r in db.M_LedgerMaster
                                   where r.GroupId != 5 && r.GroupId != 21 && r.OnWebSite == "Y"
                                   select new DeliveryAddressList
                                   {
                                     id = r.PartyCode,
                                     name = r.PartyName,                                     
                                   }).ToList();
                }
            }
            catch (Exception ex){
                throw ex;
            }
            return AddressList;
        }
    }
}
