using Newtonsoft.Json;
using SJLABSAPI.Models;
using SJLABSAPI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace SJLABSAPI.Controllers
{
    public class ApiRouterController : ApiController
    {
        UserService userservice;

        // POST: api/ApiRouter
        public Response Post([FromBody]Request request)
        {
            userservice = new UserService();
            Response response = new Response();
            if (userservice.UserExists(request.userid, request.passwd))
            {
                response.response = "OK";
            }
            else
            {
                response.response = "FAIL";
            }
            return response;
        }

        // PUT: api/ApiRouter/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiRouter/5
        public void Delete(int id)
        {
        }
    }
}
