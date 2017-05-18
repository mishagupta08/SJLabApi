 using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SJLABSAPI.Service;

namespace SJLabsAPI.Controllers
{
    public class LedgerController : ApiController
    {
        private LedgerService ledgerService;

        public LedgerController()
        {
            ledgerService = new LedgerService();
        }

        // GET api/ledger   
        [Route("getdeliveryaddresslist")]  
        [HttpPost]          
        public HttpResponseMessage getdeliveryaddresslist()
        {
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, ledgerService.GetDeliveryAddressList());
        }  

        // GET api/ledger/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/ledger
        public void Post([FromBody]string value)
        {
        }

        // PUT api/ledger/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/ledger/5
        public void Delete(int id)
        {
        }
    }
}
