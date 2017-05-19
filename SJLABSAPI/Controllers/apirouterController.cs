using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SJLABSAPI.Models;
using SJLABSAPI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace SJLABSAPI.Controllers
{
    public class apirouterController : ApiController
    {
        UserService userservice;
        LedgerService ledgerservice;
        // POST: api/ApiRouter
        public JObject Post([FromBody]Request request)
        {
            userservice = new UserService();
            ledgerservice = new LedgerService();
            string response = "{\"response\":\"FAIL\"}";
            switch (request.reqtype)
            {
                case "reqotp":
                    response = ledgerservice.getOTP(request.mobile);
                    break;
                case "getmemname":
                    response = ledgerservice.GetName(request.memberid);
                    break;
                case "getappversion":
                    response = ledgerservice.GetAppVersion();
                    break;
                case "reqlogin":
                    response = ledgerservice.checklogin(request.userid, request.passwd);
                    break;
                case "validotp":
                    response = ledgerservice.validotp(request.userid, request.passwd);
                    break;
                case "setpasswd":
                    response = ledgerservice.setpasswd(request.userid, request.passwd);
                    break;
                case "getsession":
                    response = ledgerservice.GetSession();
                    break;
            }
            return JObject.Parse(response);
        }        
    }
}
