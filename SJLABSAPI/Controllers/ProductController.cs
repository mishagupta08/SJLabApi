using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SJLABSAPI.Service;


namespace SJLabsAPI.Controllers
{
    public class ProductController : ApiController 
    {
        private ProductService productService;

        public ProductController()
        {
            this.productService = new ProductService();
        }


        // GET api/product        
        [Route("GetProductNameList")]
        [HttpPost]
        public HttpResponseMessage getproductnamelist()
        {
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, this.productService.GetProductList());
        }        

        // GET api/product/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/product
        public void Post([FromBody]string value)
        {
        }

        // PUT api/product/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/product/5
        public void Delete(int id)
        {
        }
    }
}
