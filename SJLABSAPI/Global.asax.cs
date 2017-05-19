using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using SJLabEntity;
using System.Linq;

namespace SJLABSAPI
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthorizeRequest()
        {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }

        protected void Session_Start(Object sender, EventArgs e)
        {
            //Code that runs when a new session is started     
            try
            {                  
                getData();
            }
            catch
            {
                throw;
            }
        }

        private void getData() {
            M_CompanyMaster master = null;
            m_ConfigMaster config_master = null;
            decimal sessionId=0,sessionMaster = 0;
            try {
                using (var db = new SjLabsEntities())
                {
                     master = (from r in db.M_CompanyMaster select r).FirstOrDefault();
                     config_master = (from r in db.m_ConfigMaster select r).FirstOrDefault();
                     var session = (from r in db.M_MonthlyPayDetail select r).OrderByDescending(o => o.SessID).FirstOrDefault();
                     if(session!=null)
                       sessionId = session.SessID;
                     var  session2 = (from r in db.M_SessnMaster select r).OrderByDescending(o => o.SessID).FirstOrDefault();
                     if(session2!=null)
                       sessionMaster = session2.SessID;
                }
                if (config_master != null)
                {
                    HttpContext.Current.Session["IsGetExtreme"] = config_master.IsGetExtreme;
                    HttpContext.Current.Session["IsTopUp"] = config_master.IsTopUp;
                    HttpContext.Current.Session["IsSendSMS"] = config_master.IsSendSMS;
                    HttpContext.Current.Session["IdNoPrefix"] = config_master.IdNoPrefix;
                    HttpContext.Current.Session["IsFreeJoin"] = config_master.IsfreeJoin;
                    HttpContext.Current.Session["IsStartJoin"] = config_master.IsStartJoin;
                    HttpContext.Current.Session["JoinStartFrm"] = config_master.JoinStartFrm;
                    HttpContext.Current.Session["IsSubPlan"] = config_master.IsSubPlan;
                    HttpContext.Current.Session["Logout"] = config_master.LogoutPg;
                } else
                {
                    HttpContext.Current.Session["IsGetExtreme"] = "N";
                    HttpContext.Current.Session["IsTopUp"] = "N";
                    HttpContext.Current.Session["IsSendSMS"] = "N";
                    HttpContext.Current.Session["IdNoPrefix"] = "";
                    HttpContext.Current.Session["IsFreeJoin"] = "N";
                    HttpContext.Current.Session["IsStartJoin"] = "N";
                    HttpContext.Current.Session["JoinStartFrm"] = "01-Sep-2011";
                    HttpContext.Current.Session["IsSubPlan"] = "N";
                    //HttpContext.Current.Session["Logout"] = dRead("LogoutPg");
             }
                    if (master != null)
                    {
                        HttpContext.Current.Session["CompName"] = master.CompName;
                        HttpContext.Current.Session["CompAdd"] = master.CompAdd;
                        HttpContext.Current.Session["CompWeb"] = master.WebSite == "" ? "index.asp" : master.WebSite;
                        HttpContext.Current.Session["Title"] = master.CompTitle;
                        HttpContext.Current.Session["CompMail"] = master.CompMail;
                        HttpContext.Current.Session["MailHost"] = master.MailHost;
                        HttpContext.Current.Session["MailPass"] = master.MailPass;
                        HttpContext.Current.Session["CompMobile"] = master.MobileNo;
                        HttpContext.Current.Session["ClientId"] = master.smsSenderId;
                        HttpContext.Current.Session["SmsId"] = master.smsUserNm;
                        HttpContext.Current.Session["SmsPass"] = master.smPass;
                        HttpContext.Current.Session["AdminWeb"] = master.AdminWeb;
                    }
                    else
                    {
                        HttpContext.Current.Session["CompName"] = "";
                        HttpContext.Current.Session["CompAdd"] = "";
                        HttpContext.Current.Session["CompWeb"] = "";
                        HttpContext.Current.Session["Title"] = "Welcome";
                    }
                if (sessionId != 0)
                {
                    HttpContext.Current.Session["MaxSessn"] = sessionId;
                }
                else
                {
                    HttpContext.Current.Session["MaxSessn"] = "";
                }

                if (sessionMaster != 0)
                {
                    HttpContext.Current.Session["CurrentSessn"] = sessionMaster;
                }
                else
                {
                    HttpContext.Current.Session["CurrentSessn"] = "";
                }
                
                }                                
            catch {
                HttpContext.Current.Session["CompName"] = "";
                HttpContext.Current.Session["CompAdd"] = "";
                HttpContext.Current.Session["CompWeb"] = "";
            }
        }
    }
}
