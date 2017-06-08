using System;
using System.Collections.Generic;

namespace SJLABSAPI.Models
{
    public class Request
    {
        public string userid { get; set; }
        public string reqtype { get; set; }
        public string passwd { get; set; }
        public string mobile { get; set; }
        public string memberid { get; set; }
        public string npasswd { get; set; }
        public string formno { get; set; }
        public decimal? countrycode { get; set; }
        public decimal? statecode { get; set; }      
        //member for product request       
        public List<TrnorderDetailList> trnorderdetaillist { get; set; }
        public decimal wamt { get; set; }
        public decimal repurchase { get; set; }
        public string requestfor { get; set; }
        public string remarks { get; set; }
        public string delvby { get; set; }
        public string memname { get; set; }
        public string idno { get; set; }
        public string address1 { get; set; }
        public string partycode { get; set; }
        public string chequeno { get; set; }
        public DateTime dddate { get; set; }
        public decimal paymode { get; set; }
        public string paymodetext{ get; set; }
        public decimal amount{ get; set; }        
        public DateTime chqdate { get; set; }
        public string bankname{ get; set; }
        public string issuebranch{ get; set; }
        public string filename{ get; set; }        
        public decimal bankid { get; set; }
        public int compid { get; set; } 
        public string comptype { get; set; } 
        public string complaint { get; set; } 
        public string cgroup { get; set; }
    }
   
public class TrnorderDetailList
    {
        public decimal orderno { get; set; }
        public decimal formno { get; set; }
        public decimal productid { get; set; }
        public decimal qty { get; set; }
        public decimal rate { get; set; }
        public decimal newamount { get; set; }
        public decimal rectimestamp { get; set; }
        public DateTime dispdate { get; set; }
        public string dispstatus { get; set; }        
        public decimal dispqty { get; set; }
        public decimal remqty { get; set; }
        public decimal dispamt { get; set; }       
        public string imgpath { get; set; }      
        public string fsessid { get; set; }
    }   
}