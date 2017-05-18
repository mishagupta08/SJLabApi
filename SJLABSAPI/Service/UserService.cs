using System;
using System.Collections.Generic;
using System.Linq;
using SJLabEntity;
using SJLABSAPI.Models;

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
    }
}