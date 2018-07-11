using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bootcamp_webapi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly IConfigurationRoot _config;

        public ProductsController(IConfigurationRoot config)
        {
            _config = config;
        }

        // GET api/products
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Console.WriteLine($"connection string is {_config["productdbconnstring"]}");
            Console.WriteLine($"Log level from config is {_config["loglevel"]}");
            return new[] {"product1", "product2"};
        }
    }
}

