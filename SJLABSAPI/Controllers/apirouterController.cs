﻿using Newtonsoft.Json;
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
        ApiService apiservice;
        // POST: api/ApiRouter
        public JObject Post([FromBody]Request request)
        {
            userservice = new UserService();
            apiservice = new ApiService();

            string response = "{\"response\":\"FAILED\"}";

            if (String.IsNullOrEmpty(request.reqtype))
            {
                return JObject.Parse(response);
            }

            switch (request.reqtype)
            {
                case "fillkit":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = apiservice.GetProductList();
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;
                case "getacsumm":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = apiservice.getbalance(request.userid);
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;
                case "delvptlist":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = apiservice.GetDeliveryAddressList();
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;
                case "reqotp":
                    if (!string.IsNullOrEmpty(request.mobile))
                    {
                        response = apiservice.getOTP(request.mobile);
                    }
                    break;
                case "getmemname":
                    if (!string.IsNullOrEmpty(request.memberid))
                    {
                        response = apiservice.GetName(request.memberid);
                    }
                    break;
                case "getappversion":
                    response = apiservice.GetAppVersion();
                    break;
                case "reqlogin":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd))
                    {
                        response = apiservice.checklogin(request.userid, request.passwd);
                    }
                    break;
                case "validotp":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd))
                    {
                        response = apiservice.validotp(request.userid, request.passwd);
                    }
                    break;
                case "setpasswd":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd))
                    {
                        response = apiservice.setpasswd(request.userid, request.passwd);
                    }
                    break;
                case "getsession":
                    response = apiservice.GetSession();
                    break;
                case "myteam":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = apiservice.TeamDetail(request.userid);
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;
                case "cpassword":                   
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        if (!string.IsNullOrEmpty(request.npasswd))
                        {
                            response = apiservice.ChangePassword(request.userid, request.passwd, request.npasswd);
                        }
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;
                case "forgot":
                    if (!string.IsNullOrEmpty(request.userid)) {
                        response = apiservice.forgot(request.userid);
                    }                    
                    break;
                case "country":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        response = apiservice.countrylist();
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;
                case "statelist":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        if (request.countrycode != null)
                        {
                            response = apiservice.statelist(request.countrycode);
                        }
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;
                case "citylist":
                    if (!string.IsNullOrEmpty(request.userid) && !string.IsNullOrEmpty(request.passwd) && userservice.UserExists(request.userid, request.passwd))
                    {
                        if (request.statecode!=null)
                        {
                            response = apiservice.citylist(request.statecode);
                        }
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";
                    }
                    break;

            }
            return JObject.Parse(response);
        }
    }
}
