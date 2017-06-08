using System;
using System.Collections.Generic;
using System.Linq;
using SJLabEntity;
using SJLABSAPI.Models;
using System.Net;
using System.IO;

namespace SJLABSAPI.Service
{
    public class UserService
    {
        public bool UserExists(string userId, string Password)
        {
            bool isExists = false;
            try
            {
                using (var db = new SjLabsEntities())
                {
                    var user = (from r in db.M_AppUser
                                   where r.UserID ==userId && r.OTP == Password && r.ActiveStatus == "Y"
                                   select r).FirstOrDefault();
                    if (user!=null)
                    {
                        isExists = true;
                    }
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isExists;            
        }

        public bool SendSMS(string msg, string mobileNumber)
        {
            WebClient client = new WebClient();
            string baseurl = string.Empty;
            Stream data = null;
            try
            {
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

        public bool SendMail(string MailID,string Msg,string MsgHead,string MsgType)
        {                      
            try
            {
                string _MailID = Convert.ToString(System.Web.HttpContext.Current.Session["CompMail"]);
                string _MailPass = Convert.ToString(System.Web.HttpContext.Current.Session["MailPass"]);
                string _MailHost = Convert.ToString(System.Web.HttpContext.Current.Session["MailHost"]);
                if (!string.IsNullOrEmpty(MailID) && !string.IsNullOrEmpty(_MailHost) && !string.IsNullOrEmpty(_MailID) && !string.IsNullOrEmpty(_MailPass))
                {
                    string StrMsg = string.Empty;
                    System.Net.Mail.MailAddress SendFrom = new System.Net.Mail.MailAddress(_MailID);
                    System.Net.Mail.MailAddress SendTo = new System.Net.Mail.MailAddress(MailID);
                    System.Net.Mail.MailMessage MyMessage = new System.Net.Mail.MailMessage(SendFrom, SendTo);
                    MyMessage.Subject = MsgHead;
                    MyMessage.Body = Msg;
                    MyMessage.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(_MailHost);
                    smtp.Credentials = new System.Net.NetworkCredential(_MailID, _MailPass);
                    smtp.Send(MyMessage);
                    return true;
                }
                return false;
            }        
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
                return false;
            }
        }

        public decimal GetFormNo(string userId)
        {
            decimal FrmNo = 0;
            try
            {
                using (var db = new SjLabsEntities())
                {
                    FrmNo = (from r in db.M_MemberMaster where r.IdNo == userId select r.FormNo).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException);
            }
            return FrmNo;
        }
    }
}