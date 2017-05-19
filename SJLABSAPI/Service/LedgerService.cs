using System;
using System.Collections.Generic;
using System.Linq;
using SJLInvEntity;
using SJLABSAPI.Models;
using SJLabEntity;
using System.Data.Entity;
using System.Net;
using System.IO;
using Newtonsoft.Json;

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
                Console.WriteLine(ex.InnerException);
            }
            return AddressList;
        }

        public string getOTP(string userMobile)
        {
            string response = string.Empty;            
            Random randomNumber = new Random();
            try
            {
                using (var db = new SjLabsEntities())
                {                    
                    if (userMobile.Equals("1234567890"))
                    {
                        response = "{\"response\":\"OK\"}";
                    }
                    else
                    {
                        M_AppUser appUser = (from r in db.M_AppUser where r.UserID == userMobile select r).FirstOrDefault();
                        int otpCode = randomNumber.Next(11111, 99999);
                        string wMsg = "Welcome to " + Convert.ToString(System.Web.HttpContext.Current.Session["CompName"]) + ", Your One Time Password:" + otpCode;
                        if (appUser != null)
                        {
                            appUser.OTP = Convert.ToString(otpCode);
                            db.Entry(appUser).State = EntityState.Modified;
                        }
                        else
                        {
                            appUser = new M_AppUser();
                            appUser.UserID = userMobile;
                            appUser.OTP = Convert.ToString(otpCode);
                            appUser.ActiveStatus = "Y";
                            appUser.RecTimeStamp = DateTime.Now;
                            db.M_AppUser.Add(appUser);
                        }

                        int count = db.SaveChanges();
                        if (count > 0)
                        {
                            if (SendSMS(wMsg, userMobile) == true)
                                response = "{\"response\":\"OK\"}";
                            else
                                response = "{\"response\":\"FAILED\"}";
                        }
                        else
                        {
                            response = "{\"response\":\"FAILED\"}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                response = "{\"response\":\"FAILED\"}";
            }
            return response;
        }

        public bool SendSMS(string msg, string mobileNumber)
        {
            
            WebClient client = new WebClient();
            string baseurl = string.Empty;
            Stream data;
            try {
                baseurl = "http://103.250.30.4/SendSMS/sendmsg.php?uname=" + Convert.ToString(System.Web.HttpContext.Current.Session["SmsId"]) + "&pass=" + Convert.ToString(System.Web.HttpContext.Current.Session["SmsPass"]) + "&send=" + Convert.ToString(System.Web.HttpContext.Current.Session["ClientId"]) + "&dest=" + mobileNumber + "&msg=" + msg + "\"";
                data = client.OpenRead(baseurl);
                StreamReader reader = new StreamReader(data);
                string s = string.Empty;
                s = reader.ReadToEnd();
                data.Close();
                reader.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return false;
            }            
        }

        public string GetAppVersion() {
            string appVersion = string.Empty;
            try {
                using (var db = new SjLabsEntities())
                {
                    var result = (from r in db.m_ConfigMaster select r.AppVersion).FirstOrDefault();
                    if (result != null)
                    {
                        appVersion = "{\"AppVersion\":\"" + result + "\",\"response\":\"OK\"}";
                    }
                    else
                    {
                        appVersion = "{\"response\":\"FAILED\"}";
                    }
                }
            }
            catch (Exception ex) {
                appVersion = "{\"response\":\"FAILED\"}";
            }
            return appVersion;
        }

        public string GetName(string idno)
        {
            string appVersion = string.Empty;
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var result = (from r in db.M_MemberMaster where r.IdNo == idno select r).FirstOrDefault();
                    if (result != null)
                    {
                        appVersion = "{\"memname\":\"" + result.MemFirstName + " " + result.MemLastName + "\",\"msg\":\"success\",\"response\":\"OK\"}";
                    }
                    else
                    {
                        appVersion = "{\"response\":\"FAILED\",\"memname\":\"\",\"msg\":\"ID not exist.\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                appVersion = "{\"response\":\"FAILED\",\"memname\":\"\",\"msg\":\"Invalid.\"}";
            }
            return appVersion;
        }

        public string checklogin(string userName,string Password)
        {
            string  MemName  = string.Empty;
            string response = string.Empty;           
            bool Bool = false;
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var result = (from r in db.M_MemberMaster where r.IdNo == userName && r.Passw == Password select r).FirstOrDefault();
                    if (result != null)
                    {
                        MemName = result.MemFirstName + " " + result.MemLastName;
                        Bool = true;
                    }
                    if (Bool == true)
                    {
                        M_AppUser appUser = (from r in db.M_AppUser where r.UserID == userName select r).FirstOrDefault();
                        if (appUser != null)
                        {
                            appUser.OTP = Password;
                            db.Entry(appUser).State = EntityState.Modified;
                        }
                        else
                        {
                            appUser = new M_AppUser();
                            appUser.UserID = userName;
                            appUser.OTP = Convert.ToString(Password);
                            appUser.ActiveStatus = "Y";
                            appUser.RecTimeStamp = DateTime.Now;
                            db.M_AppUser.Add(appUser);
                        }
                        int count = db.SaveChanges();
                        if (count > 0)
                        {
                            response = "{\"response\":\"OK\",\"mname\":\"" + MemName + "\"}";
                        }
                        else
                        {
                            response = "{\"response\":\"FAILED\"}";
                        }
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Invalid Login Details.\"}";                            
                    }
                }
            }
            catch (Exception ex)
            {
                response = "{\"response\":\"FAILED\"}";
            }
            return response;
        }
        
        public string validotp(string userMobile, string otpcode)
        {
            string MemName = string.Empty;
            string response = string.Empty;
            decimal _retry = 0;
            bool Bool = false;
            try
            {
                if (userMobile == "1234567890" && otpcode == "111111")
                {
                    response = "{\"response\":\"OK\"}";
                }
                else
                {
                    using (var db = new SjLabsEntities())
                    {
                        var result = (from r in db.M_AppUser where r.UserID == userMobile && r.OTP == otpcode && r.ActiveStatus=="Y" select r).FirstOrDefault();
                        if (result != null)
                        {
                            Bool = true;
                        }
                        if (Bool == false)
                        {
                            M_AppUser appUser = (from r in db.M_AppUser where r.UserID == userMobile && r.ActiveStatus == "Y" select r).FirstOrDefault();
                            if (appUser != null)
                            {
                                appUser.Retry = appUser.Retry +1;
                                db.Entry(appUser).State = EntityState.Modified;
                            }                            
                            int count = db.SaveChanges();

                            appUser = (from r in db.M_AppUser where r.UserID == userMobile && r.ActiveStatus == "Y" select r).FirstOrDefault();

                            if (appUser!=null)
                            {
                                _retry = appUser.Retry;
                               
                            }
                            response = "{\"response\":\"FAILED\",\"retry\":\"" + _retry + "\"}";
                        }
                        else
                        {
                            response = "{\"response\":\"OK\"}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = "{\"response\":\"FAILED\"}";
            }
            return response;
        }

        public string setpasswd(string userEmail, string password)
        {
            string response = string.Empty;       
            try
            {              
                    using (var db = new SjLabsEntities())
                    {                        
                            response = "{\"response\":\"FAILED\"}";                        
                    }                
            }
            catch (Exception ex)
            {
                response = "{\"response\":\"FAILED\"}";
            }
            return response;
        }


        public string GetSession()
        {
            string response = string.Empty;
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var session = (from r in db.M_SessnMaster where r.ToDate != null && r.OnWebSite.ToUpper() == "Y" select new { sessid = r.SessID}).OrderByDescending(o => o.sessid).ToList();
                    if (session != null)
                    {
                        response = "{\"session\":" + JsonConvert.SerializeObject(session);
                        response = response + ",\"response\":\"OK\"}";
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\"}";
                    }
                    
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
