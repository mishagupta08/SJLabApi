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
using System.Data.SqlClient;

namespace SJLABSAPI.Service
{
    public class ApiService
    {
        string sConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SJLABApplicationServices"].ConnectionString;
        UserService uService = null;
        SqlConnection conn = null;
        SqlCommand sqlCmd = null;
        SqlDataReader dr = null;

        public string GetDeliveryAddressList()
        {
            string response = "{\"response\":\"FAILED\"}";
            List<DeliveryAddressList> AddressList = new List<DeliveryAddressList>();
            try
            {
                using (var db = new SJLInvEntities())
                {
                    AddressList = (from r in db.M_LedgerMaster
                                   where r.GroupId != 5 && r.GroupId != 21 && r.OnWebSite == "Y"
                                   select new DeliveryAddressList
                                   {
                                       delvcode = r.PartyCode,
                                       centername = r.PartyName,
                                   }).ToList();
                    if (AddressList != null)
                    {
                        response = "{\"delvpoint\":" + JsonConvert.SerializeObject(AddressList) + ",\"response\":\"OK\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
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
                            if (uService.SendSMS(wMsg, userMobile) == true)
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

        

        public string GetAppVersion()
        {
            string appVersion = string.Empty;
            try
            {
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
            catch (Exception ex)
            {
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
                appVersion = "{\"response\":\"FAILED\"}";
            }
            return appVersion;
        }

        public string checklogin(string userName, string Password)
        {
            string MemName = string.Empty;
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
                        var result = (from r in db.M_AppUser where r.UserID == userMobile && r.OTP == otpcode && r.ActiveStatus == "Y" select r).FirstOrDefault();
                        if (result != null)
                        {
                            Bool = true;
                        }
                        if (Bool == false)
                        {
                            M_AppUser appUser = (from r in db.M_AppUser where r.UserID == userMobile && r.ActiveStatus == "Y" select r).FirstOrDefault();
                            if (appUser != null)
                            {
                                appUser.Retry = appUser.Retry + 1;
                                db.Entry(appUser).State = EntityState.Modified;
                            }
                            int count = db.SaveChanges();

                            appUser = (from r in db.M_AppUser where r.UserID == userMobile && r.ActiveStatus == "Y" select r).FirstOrDefault();

                            if (appUser != null)
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
                    var session = (from r in db.M_SessnMaster where r.ToDate != null && r.OnWebSite.ToUpper() == "Y" select new { sessid = r.SessID }).OrderByDescending(o => o.sessid).ToList();
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

        public string TeamDetail(string userid)
        {
            string response = "{\"response\":\"FAILED\"}";
            uService = new UserService();
            try
            {                                  
                decimal FormNo = uService.GetFormNo(userid);
                using (conn = new SqlConnection(sConnectionString))
                {
                    conn.Open();
                    sqlCmd = new SqlCommand("Select * FROM MyTeamData(" + FormNo + ")", conn);
                    dr = sqlCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        response = "{\"sessXreg\":\"" + dr["RegisterLeft"] + "\",\"sessYreg\":\"" + dr["RegisterRight"] + "\",\"sessTreg\":\"" + (Convert.ToInt32(dr["RegisterLeft"]) + Convert.ToInt32(dr["RegisterRight"])) + "\",\"sessXconf\":\"" + dr["ConfirmLeft"] + "\",\"sessYconf\":\"" + dr["ConfirmRight"] + "\",\"sessTconf\":\"" + (Convert.ToInt32(dr["ConfirmLeft"]) + Convert.ToInt32(dr["ConfirmRight"])) + "\",\"totalXreg\":\"" + dr["TRegisterLeft"] + "\",\"totalYreg\":\"" + dr["TRegisterRight"] + "\",\"totalTreg\":\"" + (Convert.ToInt32(dr["TRegisterLeft"]) + Convert.ToInt32(dr["TRegisterRight"])) + "\",\"totalXconf\":\"" + dr["TConfirmLeft"] + "\",\"totalYconf\":\"" + dr["TConfirmRight"] + "\",\"totalTconf\":\"" + (Convert.ToInt32(dr["TConfirmLeft"]) + Convert.ToInt32(dr["TConfirmRight"])) + "\",\"legXcf\":\"" + dr["LegXBVCF"] + "\",\"legYcf\":\"" + dr["LegYBVCF"] + "\",\"legTcf\":\"" + (Convert.ToInt32(dr["LegXBVCF"]) + Convert.ToInt32(dr["LegYBVCF"])) + "\",\"sessXbv\":\"" + dr["BinaryXSessBV"] + "\",\"sessYbv\":\"" + dr["BinaryYSessBV"] + "\",\"sessTbv\":\"" + (Convert.ToInt32(dr["BinaryXSessBV"]) + Convert.ToInt32(dr["BinaryYSessBV"])) + "\",\"totalXbv\":\"" + dr["BinaryXBV"] + "\",\"totalYbv\":\"" + dr["BinaryYBV"] + "\",\"totalTbv\":\"" + (Convert.ToInt32(dr["BinaryXBV"]) + Convert.ToInt32(dr["BinaryYBV"])) + "\" ,\"response\":\"OK\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }
       
        public string GetProductList()
        {
            string response = "{\"response\":\"FAILED\"}";
            List<ProductNameList> productList = new List<ProductNameList>();
            try
            {
                using (var db = new SJLInvEntities())
                {
                    productList = (from r in db.M_ProductMaster
                                   where r.ActiveStatus == "Y" && r.OnWebSite == "Y"
                                   select new ProductNameList
                                   {
                                       id = r.ProdId,
                                       name = r.ProductName,
                                       MRP = r.MRP,
                                       BV = r.BV,
                                       DP = r.DP,
                                       RP = r.RP
                                   }).OrderBy(o => o.name).ToList();
                    if (productList != null)
                    {
                        response = "{\"products\":" + JsonConvert.SerializeObject(productList) + ",\"response\":\"OK\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string getbalance(string userId)
        {
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                using (conn = new SqlConnection(sConnectionString))
                {
                    conn.Open();
                    sqlCmd = new SqlCommand("Select * From dbo.ufnGetBalance('" + userId + "', 'M')", conn);
                    conn.Open();
                    dr = sqlCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        response = "{\"credit\":\"" + dr["Credit"] + "\",\"debit\":\"" + dr["Debit"] + "\",\"balance\":\"" + dr["Balance"] + "\",\"response\":\"OK\"}";
                    }
                    dr.Close();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                dr.Close();
                conn.Close();
            }
            return response;
        }

        public string ChangePassword(string userId, string oldpwd, string newpwd)
        {
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                using (var db=new SjLabsEntities())
                {
                    var chkUser = (from r in db.M_MemberMaster where r.IdNo == userId && r.Passw == oldpwd select r).FirstOrDefault();
                    if (chkUser != null)
                    {
                        string strQry = "Insert Into TempMemberMaster Select *,'Password updated through App',GetDate(),'C' From M_MemberMaster Where IDNo='" + userId + "';Update M_MemberMaster Set Passw='" + newpwd + "' Where IDNO='" + userId + "';Update M_AppUser Set OTP='" + newpwd + "' Where UserID='" + userId + "'";                        
                        using (conn = new SqlConnection(sConnectionString))
                        {
                            conn.Open();
                            sqlCmd = new SqlCommand(strQry, conn);
                            int i = sqlCmd.ExecuteNonQuery();
                            if (i != 0)
                            {
                                response = "{\"response\":\"OK\"}";
                            }
                        }                        
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"msg\":\"Incorrect Password\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);                
            }
            return response;
        }

        public string forgot(String userID)
        {
            string response = "{\"response\":\"FAILED\"}";
            uService = new UserService();
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var user = (from r in db.M_MemberMaster where r.IdNo == userID select r).FirstOrDefault();
                    if (user != null)
                    {
                        string userEmail = user.EMail;
                        string userMobl = Convert.ToString(user.Mobl);
                        string sms = "Dear " + user.MemFirstName.Trim() + ", Your login details are ID-" + user.IdNo + "/ Pwd-" + user.Passw + "/Trans Code- " + user.EPassw + ", pls visit " + Convert.ToString(System.Web.HttpContext.Current.Session["CompWeb"]) + " for more details.";
                        string wMsg = "<table style='margin:0; padding:10px; font-size:12px; font-family:Verdana, Arial, Helvetica, sans-serif; line-height:23px; text-align:justify;width:100%'> ";
                        wMsg = wMsg + "<tr><td>";
                        wMsg = wMsg + "<span style='color: #0099CC; font-weight: bold;'><h2>Dear " + user.MemFirstName + ",</h2></span><br />< br /> ";
                        wMsg = wMsg + "Your username and password are found below : <br />";
                        wMsg = wMsg + "<span style='color: #0099FF; font-weight: bold;'></span><br />";
                        wMsg = wMsg + "<strong>Use your ID No. as username for login.</strong><br />";
                        wMsg = wMsg + "<strong>Password: " + user.Passw + "</strong><br />";
                        wMsg = wMsg + "You may login at: <a href='" + Convert.ToString(System.Web.HttpContext.Current.Session["CompWeb"]) + "' target='_blank' style='color:#0000FF; text-decoration:underline;'>" + Convert.ToString(System.Web.HttpContext.Current.Session["CompWeb"]) + "</a><br />";
                        wMsg = wMsg + "<br />";
                        wMsg = wMsg + "<span style='color: #0099FF; font-weight: bold;'>Sincerely,</span><br />";
                        wMsg = wMsg + "<a href=' " + Convert.ToString(System.Web.HttpContext.Current.Session["CompWeb"]) + "' target='_blank' style='color:#0000FF; text-decoration:underline;'>" + Convert.ToString(System.Web.HttpContext.Current.Session["CompWeb"]) + "</a><br />";
                        wMsg = wMsg + "<br /><br /></td></tr></table>";
                        string isSmsSent = "N", isMailSent = "N";
                        string _MailHead = "Your " + Convert.ToString(System.Web.HttpContext.Current.Session["CompName"]) + " Login Password";
                        if (!string.IsNullOrEmpty(userEmail))
                        {
                            if (uService.SendMail(userEmail, wMsg, _MailHead, "FORGOT"))
                            {
                                isMailSent = "Y";
                            }
                            if (uService.SendSMS(sms, userMobl))
                            {
                                isSmsSent = "Y";
                            }
                        }
                        response = "{\"response\":\"OK\",\"isuser\":\"Y\",\"ismailsent\":\"" + isMailSent + "\",\"issmssent\":\"" + isSmsSent + "\"}";
                    }
                    else
                    {
                        response = "{\"response\":\"FAILED\",\"isuser\":\"N\"}";
                    }
                }                                                                                                                                             
            }
           catch(Exception ex){
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string countrylist()
        {
            string response = "{\"response\":\"FAILED\"}";          
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var countryList = (from r in db.M_CountryMaster
                                   where r.ActiveStatus == "Y"
                                   select new {
                                       countrycode = r.CountryCode,
                                       countryname = r.CountryName
                                   }).OrderBy(o => o.countryname).ToList();
                    if (countryList != null)
                    {
                        response = "{\"countries\":" + JsonConvert.SerializeObject(countryList) + ",\"response\":\"OK\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string statelist(decimal? CountryCode)
        {
            string response = "{\"response\":\"FAILED\"}";            
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var stateList = (from r in db.M_StateDivMaster
                                   where r.ActiveStatus == "Y" && r.CountryCode ==CountryCode
                                   select new {
                                       statecode = r.StateCode,
                                       statename = r.StateName
                                   }).OrderBy(o => o.statename).ToList();
                    if (stateList != null)
                    {
                        response = "{\"states\":" + JsonConvert.SerializeObject(stateList) + ",\"response\":\"OK\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string citylist(decimal? StateCode)
        {
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var citylist = (from r in db.M_CityStateMaster join  s in db.M_DistrictMaster on r.DistrictCode equals s.DistrictCode
                                    where r.ActiveStatus == "Y" && s.DistrictCode ==StateCode
                                     select new
                                     {
                                         citycode = r.CityCode,
                                         cityname = r.CityName
                                     }).OrderBy(o => o.cityname).ToList();
                    if (citylist != null)
                    {
                        response = "{\"cities\":" + JsonConvert.SerializeObject(citylist) + ",\"response\":\"OK\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

       
                

    }
}