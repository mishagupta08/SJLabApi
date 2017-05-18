using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace SJLABSAPI.Models
{
    public class Request
    {
        public string userid { get; set; }
        public string reqtype { get; set; }
        public string passwd { get; set; }
    }
}