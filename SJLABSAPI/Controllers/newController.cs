using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SJLABSAPI.Controllers
{
    public class newController : ApiController
    {
        // GET: api/new
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/new/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/new
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/new/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/new/5
        public void Delete(int id)
        {
        }
    }
}
