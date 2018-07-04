using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Pivotal.Discovery.Client;

public class HomeController: Controller {

    //private static HttpClient client = new HttpClient();
    DiscoveryHttpClientHandler handler;

    public HomeController(IDiscoveryClient client) {
        handler = new DiscoveryHttpClientHandler(client);
    }

    [Route("/home")]
    public IActionResult Index() {

        var client = GetClient();
        var result = client.GetStringAsync("https://<replace me>/api/products").Result;

        ViewData["products"] = result;
        return View();
    }

    private HttpClient GetClient() {
        var client = new HttpClient(handler, false);
        return client;
    }
}