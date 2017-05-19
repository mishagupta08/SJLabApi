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
        ProductService productservice;
        // POST: api/ApiRouter
        public JObject Post([FromBody]Request request)
        {
            userservice = new UserService();
            ledgerservice = new LedgerService();
            productservice = new ProductService();
            string response = "{\"response\":\"FAILED\"}";
            switch (request.reqtype)
            {
                case "fillkit":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = productservice.GetProductList();
                    }                                        
                    break;
                case "fillbalance":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && !string.IsNullOrEmpty(request.formno) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = productservice.getbalance(request.formno);
                    }
                    break;
                case "filldeliverycenter":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = ledgerservice.GetDeliveryAddressList();
                    }
                    break;
                case "reqotp":
                    if (!string.IsNullOrEmpty(request.mobile))
                    {
                        response = ledgerservice.getOTP(request.mobile);
                    }
                    break;
                case "getmemname":
                    if (!string.IsNullOrEmpty(request.memberid))
                    {
                        response = ledgerservice.GetName(request.memberid);
                    }
                    break;
                case "getappversion":
                    response = ledgerservice.GetAppVersion();
                    break;
                case "reqlogin":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd))
                    {
                        response = ledgerservice.checklogin(request.userid, request.passwd);
                    }
                    break;
                case "validotp":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd))
                    {
                        response = ledgerservice.validotp(request.userid, request.passwd);
                    }
                    break;
                case "setpasswd":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd))
                    {
                        response = ledgerservice.setpasswd(request.userid, request.passwd);
                    }
                    break;
                case "getsession":
                    response = ledgerservice.GetSession();
                    break;
                case "myteam":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd))
                    {
                        response = ledgerservice.TeamDetail(request.userid, request.passwd);
                    }
                    break;
                case "cpassword":
                    //response = ledgerservice.ChangePassword(request.userid, request.passwd,request.npasswd);
                    break;  
                                      
            }
            return JObject.Parse(response);
        }        
    }
}
