using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bootcamp_webapi.Controllers
{
    [Route("api/[controller]")]
    public class FailureController : Controller
    {
        public FailureController(IConfigurationRoot config)
        {
            Config = config;
        }

        private IConfigurationRoot Config { get; set; }

        // GET api/failure
        [HttpGet]
        public ActionResult TriggerFailure(string action)
        {
            System.Console.WriteLine("bombing out ...");
            //purposely crash
            System.Environment.Exit(100);
            return null;
        }
    }
}

