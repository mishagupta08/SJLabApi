using System;
using System.Collections.Generic;
using System.Linq;
using SJLInvEntity;
using SJLABSAPI.Models;
using SJLabEntity;
using System.Data.Entity;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Web;

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

        public string FillRepurchaseBalance(string userid)
        {
            uService = new UserService();
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                decimal formno = uService.GetFormNo(userid);
                response = "{\"balance\":\"" + getTypeBalance(formno, "R") + "\",\"response\":\"OK\"}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string FillMainBalance(string userid)
        {
            uService = new UserService();
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                decimal formno = uService.GetFormNo(userid);
                response = "{\"balance\":\"" + getTypeBalance(formno, "M") + "\",\"response\":\"OK\"}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        private decimal getTypeBalance(decimal formno, string type)
        {
            uService = new UserService();
            decimal response = 0;
            try
            {
                using (conn = new SqlConnection(sConnectionString))
                {
                    conn.Open();
                    sqlCmd = new SqlCommand("Select * From dbo.ufnGetBalance('" + formno + "','" + type + "')", conn);
                    dr = sqlCmd.ExecuteReader();
                    if (dr.Read())
                    {
                        response = Convert.ToDecimal(dr["Balance"]);
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
                using (var db = new SjLabsEntities())
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
            catch (Exception ex)
            {
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
                                       select new
                                       {
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
                                     where r.ActiveStatus == "Y" && r.CountryCode == CountryCode
                                     select new
                                     {
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
                    var citylist = (from r in db.M_CityStateMaster
                                    join s in db.M_DistrictMaster on r.DistrictCode equals s.DistrictCode
                                    where r.ActiveStatus == "Y" && s.DistrictCode == StateCode
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

        public string PaymentDetails(string userid)
        {
            string response = "{\"response\":\"FAILED\"}";
            uService = new UserService();
            try
            {
                using (var db = new SjLabsEntities())
                {
                    decimal formno = uService.GetFormNo(userid);
                    var list = (from r in db.TrnOrders
                                where r.ORDERTYPE.ToUpper() == "O" && r.FormNo == formno
                                select
new
{
orderno = r.OrderNo,
orderdate = r.OrderDate,
orderqty = r.OrderQty,
ordeeramt = r.OrderAmt,
bankamt = r.BankAmt,
otheramt = r.OtherAmt,
walletamt = r.WalletAmt,
remark = r.Remark,
status = r.DispatchStatus == "Y" ? "Dispatched" : "Pending",
}).ToList();
                    response = "{\"orders\":" + JsonConvert.SerializeObject(list) + ",\"response\":\"OK\"}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string productrequest(Request productrequest)
        {
            string response = "{\"response\":\"FAILED\"}";
            uService = new UserService();
            try
            {
                decimal formNo = uService.GetFormNo(productrequest.idno);
                decimal FSessId = 0;
                decimal orderno = 100001;
                decimal TotalOrder = 0;
                decimal TotalQty = 0;
                decimal TotalAmount = 0;
                decimal Ledgerid = 0;
                decimal RBalance = 0;
                decimal MBalance = 0;
                decimal TAmount = 0;
                int c = 0;
                string query = string.Empty;
                using (var db = new SJLInvEntities())
                {
                    var FSess = (from r in db.M_FiscalMaster select r).OrderByDescending(o => o.FSessId).FirstOrDefault();
                    FSessId = FSess.FSessId;
                }

                using (var db = new SjLabsEntities())
                {
                    var order = (from r in db.TrnOrders where r.ORDERTYPE == "O" select r).OrderByDescending(o => o.OrderNo).FirstOrDefault();
                    if (order != null)
                    {
                        orderno = order.OrderNo + 1;
                    }

                    var Ledger = (from r in db.Ac_LedgerMaster where r.LedgerName == productrequest.partycode && r.ActiveStatus == "Y" select r).FirstOrDefault();
                    if (Ledger != null)
                    {
                        Ledgerid = Ledger.LedgerID;
                    }
                }

                if (productrequest.requestfor.ToUpper() == "S")
                {
                    RBalance = getTypeBalance(formNo, "R");
                }

                MBalance = getTypeBalance(formNo, "M");

                M_ProductMaster product = null;
                foreach (TrnorderDetailList orderrow in productrequest.trnorderdetaillist)
                {
                    product = GetProductDetail(orderrow.productid);
                    query += "Insert Into TrnorderDetail(OrderNo,FormNo,ProductID,Qty,Rate,NetAmount,RecTimeStamp,DispDate,DispStatus,DispQty,RemQty,DispAmt,MRP,DP,ProductName,ImgPath,RP,BV,FSEssId)";
                    query += " Values('" + orderno + "','" + formNo + "','" + orderrow.productid + "','" + orderrow.qty + "','" + orderrow.rate + "','" + (product.DP * orderrow.qty) + "',Getdate(),'','P',0,'" + orderrow.qty + "',0,";
                    query += " '" + product.MRP + "','" + product.DP + "','" + product.ProductName + "','','" + (product.RP * orderrow.qty) + "','" + (product.BV * orderrow.qty) + "','1' )";
                    TotalOrder = TotalOrder + 1;
                    TotalAmount = TotalAmount + (product.DP * orderrow.qty);
                    TotalQty = TotalQty + orderrow.qty;
                    c = c + 1;
                }

                if (productrequest.wamt > 0)
                {
                    query = query + "INSERT INTO TrnVoucher(VoucherNo,VoucherDate,DrTo,CrTo,Amount,Narration,RefNo,AcType,VTYpe,SessID,WSessID) SELECT ISNULL(Max(VoucherNo)+1,1001),'" + DateTime.Now.ToString("dd-MMM-yyyy") + "','";
                    query = query + formNo + "','0'," + productrequest.wamt + ",'Amount deducted by Product Request Req.No.:" + orderno + ".','Req/" + formNo + "','M','D',Convert(Varchar,Getdate(),112),'" + Convert.ToString(HttpContext.Current.Session["CurrentSessn"]) + "' FROM TrnVoucher;";
                }

                if (productrequest.requestfor.ToUpper() == "S" && productrequest.repurchase > 0)
                {
                    query = query + "INSERT INTO TrnVoucher(VoucherNo,VoucherDate,DrTo,CrTo,Amount,Narration,RefNo,AcType,VTYpe,SessID,WSessID) SELECT ISNULL(Max(VoucherNo)+1,1001),'" + DateTime.Now.ToString("dd-MMM-yyyy") + "','";
                    query = query + formNo + "','0'," + productrequest.repurchase + ",'Amount deducted by Product Request Req.No.:" + orderno + ".','Req/" + formNo + "','R','D',Convert(Varchar,Getdate(),112),'" + Convert.ToString(HttpContext.Current.Session["CurrentSessn"]) + "' FROM TrnVoucher;";
                }


                query = query + "Insert INTO TrnOrder(OrderNo,OrderDate,MemFirstName,MemLastName,Address1,Address2,CountryID,CountryName,StateCode,City,PinCode,";
                query = query + " Mobl,EMail,FormNo,UserType,Passw,PayMode,ChDDNo,ChDate,ChAmt,BankName,BranchName,Remark,OrderAmt,OrderItem,";
                query = query + " OrderQty,ActiveStatus,HostIp,RecTimeStamp,IsTransfer,DispatchDate,DispatchStatus,DispatchQty,RemainQty,";
                query = query + " DispatchAmount,Shipping,SessID,RewardPoint,CourierName,DocketNo,OrderFor,IsConfirm,OrderType,Discount,OldShipping,ShippingStatus,IdNo,FSessId,BankAmt,OtherAmt,WalletAmt)";
                query = query + " select '" + orderno + "',Cast(Convert(varchar,GETDATE(),106) as Datetime),MemFirstName , MemLastName , '" + productrequest.address1 + "' , Address2 , CountryID , CountryName , StateCode , City , Case when PinCode='' then 0 else Pincode  end as Pincode ,";
                query = query + " Mobl, EMail ,'" + formNo + "','', Passw ,'',0,'',0,'','','" + productrequest.remarks + "','" + TotalAmount + "','" + TotalOrder + "','" + TotalQty + "',";
                query = query + "'Y','" + productrequest.delvby + "',Getdate(),'Y','','N',0,'" + TotalQty + "',0,0,'" + Convert.ToString(HttpContext.Current.Session["CurrentSessn"]) + "',0,'',0,'" + productrequest.partycode + "','Y','O',0," + formNo + ",'Y','" + productrequest.idno + "','" + FSessId + "','0','" + productrequest.repurchase + "','" + productrequest.wamt + "' from M_memberMaster where formno='" + formNo + "'";

                query = query + " insert into UserHistory(UserId,UserName,PageName,Activity,ModifiedFlds,RecTimeStamp,Memberid)Values";
                query = query + "('" + formNo + "','" + productrequest.memname + "','Product Request','Product Request',' Product Request For Order No " + orderno + " ',Getdate()," + formNo + ")";

                query = query + " Insert into SJLInv..TrnPaymentConfirmation(SNo,ConfirmBy,OrderNo,FormNo,OrderAmt,IsConfirm,RecTimeStamp,UserID,OrderFor,";
                query = query + " IDNO,ActiveStatus,OrdType,FSessId)select Case When Max(SNo) Is Null Then '1001' Else Max(SNo)+1 END as SNo,'WR','" + orderno + "',";
                query = query + "  '" + formNo + "','" + TotalAmount + "','Y',Getdate(),0,'WR','" + productrequest.idno + "','Y','D',1 from  " + "SJLInv..TrnPaymentConfirmation";

                query += " insert into Ac_TrnVoucher(VoucherID, VoucherDate, LedgerID, VTID, CrAmt, DrAmt, FrmLedgerID, Paymode, RefNo, ActiveStatus, RecTimeStamp)";
                query += " select Case When Max(VoucherId) Is Null Then '1' Else Max(VoucherId)+1 END as VoucherId ,Getdate(),'" + formNo + "','8',0,'" + TotalAmount + "'," + Ledgerid + ",";
                query += "'','" + Convert.ToString(HttpContext.Current.Session["CurrentSessn"]) + "/" + formNo + "','Y',Getdate() from Ac_TrnVoucher ";

                query = query + " Exec " + "SJLInv..Dispatchorder " + orderno + ";";

                using (conn = new SqlConnection(sConnectionString))
                {
                    conn.Open();
                    sqlCmd = new SqlCommand(query, conn);
                    int i = sqlCmd.ExecuteNonQuery();
                    if (i != 0)
                    {
                        response = "{\"response\":\"OK\"}";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public M_ProductMaster GetProductDetail(decimal productId)
        {
            M_ProductMaster product = new M_ProductMaster();
            try
            {
                using (var db = new SJLInvEntities())
                {
                    product = (from r in db.M_ProductMaster where r.ActiveStatus == "Y" && r.OnWebSite == "Y" select r).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return product;
        }

        public string WalletRequestDetail(string userid)
        {
            string response = "{\"response\":\"FAILED\"}";
            uService = new UserService();
            try
            {
                using (var db = new SjLabsEntities())
                {
                    decimal formno = uService.GetFormNo(userid);
                    var list = (from r in db.WalletReqs
                                join s in db.M_BankMaster on r.BankId equals s.BankCode
                                where s.RowStatus.ToUpper() == "Y" && r.Formno == formno
                                select new
                                {
                                    reqno = r.ReqNo,
                                    reqdate = r.ReqDate,
                                    paymode = r.Paymode,
                                    chqno = r.ChqNo,
                                    chqdate = r.ChqDate,
                                    bankname = s.BankName,
                                    branchname = s.BranchName,
                                    status = r.IsApprove.ToUpper() == "N" ? "Pending" : r.IsApprove == "Y" ? "Approve" : "Rejected",
                                    amount = r.Amount,
                                    remark = s.Remarks,
                                    scannedfile = r.ScannedFile == "" ? "" : "images/UploadImage/" + r.ScannedFile,
                                    scannedfilestatus = r.ScannedFile == "" ? false : true,
                                }).ToList();
                    response = "{\"paymentdetails\":" + JsonConvert.SerializeObject(list) + ",\"response\":\"OK\"}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string FillPaymode()
        {
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var list = (from r in db.M_PayModeMaster
                                where r.ActiveStatus == "Y"
                                select new
                                {
                                    pid = r.PId,
                                    paymode = r.PayMode
                                }).ToList();
                    response = "{\"paymodes\":" + JsonConvert.SerializeObject(list) + ",\"response\":\"OK\"}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string FillBankMaster()
        {
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var list = (from r in db.M_BankMaster
                                where r.ActiveStatus == "Y" && r.RowStatus == "Y" && r.BankCode == 1
                                select new
                                {
                                    code = r.BankCode,
                                    name = r.BankName
                                }).ToList();
                    response = "{\"banks\":" + JsonConvert.SerializeObject(list) + ",\"response\":\"OK\"}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string checkChequeExistance(string chequeNo)
        {
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                using (var db = new SjLabsEntities())
                {

                    var list = (from r in db.WalletReqs
                                where r.ChqNo == chequeNo
                                select r).ToList();

                    if (list != null && list.Count > 0)
                    {
                        response = "{\"isExists\":\"true\",\"response\":\"OK\"}";
                    }
                    else
                    {
                        response = "{\"isExists\":\"false\",\"response\":\"OK\"}";
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }


        
            public string SaveWalletRequest(Request request)
        {
            string response = "{\"response\":\"FAILED\"}";
            string query = string.Empty;
            uService = new UserService();
            decimal formNo = uService.GetFormNo(request.userid);
            DateTime ChqDate = DateTime.Now;
            if (request.dddate != null)
            {
                ChqDate = request.dddate;
            }
            decimal reqno = 0;
            int i = 0;            

            try
            {
                query = "INSERT INTO WalletReq(ReqNo, ReqDate, Formno, PID, Paymode, Amount, ChqNo, ChqDate, BankName, BranchName, ScannedFile, Remarks, BankId, Transno)  ";
                query += "Select ISNULL(Max(ReqNo)+1,'1001'),getDate(),'" + formNo + "','" + request.paymode + "','" + request.paymodetext + "','" + request.amount + "',";
                query += "'" + request.chequeno + "','"+ ChqDate + "','" + request.bankname + "','" + request.issuebranch + "','" + request.filename + "','" + request.remarks + "','" + request.bankid + "','" + request.chequeno + "' FROM WalletReq ";
                query += "; Insert into UserHistory(UserId,UserName,PageName,Activity,ModifiedFlds,RecTimeStamp,MemberId)Values";
                query += "('" + formNo + "','" + request.memname + "','Payment Request','Payment Request','Amount: " + request.amount + "',Getdate()," + formNo + ")";

                using (conn = new SqlConnection(sConnectionString))
                {
                    conn.Open();
                    sqlCmd = new SqlCommand(query, conn);
                    i = sqlCmd.ExecuteNonQuery();
                }

                using (var db = new SjLabsEntities())
                {
                    if (i > 0)
                    {
                        reqno = (from r in db.WalletReqs where r.Formno == formNo && r.Amount == request.amount select r.Amount).FirstOrDefault();
                        string msg = "Payment request with  ReqNo=" + reqno + ";Idno=" + request.idno + ";Name:" + request.memname + ";Amount:" + request.amount;
                        if (uService.SendSMS("8302530036", msg))
                        {
                            response = "{\"response\":\"OK\"}";

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return response;
        }

        public string GetFormNo(string userId)
        {
            string response = "{\"response\":\"FAILED\"}";
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var record = (from r in db.M_MemberMaster
                                  where r.IdNo == userId
                                  select new
                                  {
                                      formno = r.FormNo,
                                      fname = r.MemFirstName,
                                      lanme = r.MemLastName
                                  }).FirstOrDefault();
                    response = "{\"formno\":\"" + record.formno + "\",\"memname\":\"" + record.fname + " " + record.lanme + "\",\"response\":\"OK\"}";
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