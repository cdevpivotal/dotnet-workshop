 <h1 id="introduction">Introduction</h1>
<p>It’s fun to try out the platform itself and see how it works in certain
situations. Here we trigger autoscaling, and observe failure recovery.</p>
<h1 id="configure-and-test-out-autoscaling-policies">Configure and test out autoscaling policies</h1>
<p>Autoscaling is a key part of any solid platform. Here we create a
policy, and trigger it!</p>
<ol>
<li>
<p>Download the .NET Core project located at
<a href="https://github.com/cdevpivotal/dotnet-workshop/tree/master/cloud-native-net-loadtest" rel="noreferrer noopener">https://github.com/cdevpivotal/dotnet-workshop/tree/master/cloud-native-net-loadtest</a>.</p>
</li>
<li>
<p>Open the project in Visual Studio Code.</p>
</li>
<li>
<p>Update the <code>Program.cs</code> file with a pointer to your API microservice
URL.</p>
</li>
<li>
<p>In Apps Manager, go to the Marketplace and provision the <code>Autoscaler</code>
service.</p>
</li>
<li>
<p>Bind the autoscaler service to your microservice by viewing the
autoscaler service, add choosing to add a bound app.</p>
</li>
<li>
<p>Click the &quot;manage&quot; link on the autoscaler page to set up a scaling
policy.</p>
</li>
<li>
<p>Click &quot;edit&quot; on Instance Limits and set it to have a minimum of 1,
and maximum of 3.</p>
</li>
<li>
<p>Edit the scaling rules and set an <code>HTTP Throughput</code> policy that
scales down if less than 1 request per second, and scales up if more
than 4 requests per second.</p>
</li>
<li>
<p>Enable the policy by sliding the toggle at the top of the policy
definition. Save the policy.</p>
</li>
<li>
<p>Start the &quot;load test&quot; .NET Core project on your machine that
repeatedly calls your microservice. Start it by entering <code>dotnet run</code>
in the Terminal while pointing at that application folder.</p>
</li>
<li>
<p>On the overview page of your microservice in Apps Manager, observe a
second instance come online shortly. This is in response to the elevated
load, and your autoscaling policy kicking in. Also notice a new &quot;event&quot;
added to the list.</p>
</li>
<li>
<p>Stop the load testing app (Ctrl+C), and watch the application scale
back down to a single instance within 30 seconds.</p>
</li>
</ol>
<h1 id="update-microservice-with-new-fault-endpoint-and-logs">Update microservice with new &quot;fault&quot; endpoint and logs</h1>
<p>Let’s add a new microservice endpoint that purposes crashes our app.
We can see how the platform behaves when an instance disappears.</p>
<ol>
<li>
<p>Return to your products API microservice in Visual Studio Code.</p>
</li>
<li>
<p>Create a new controller named <code>FailureController.cs</code> with the
following content:</p>
<pre><code class="language-csharp">using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace bootcamp_webapi.Controllers
{
    [Route(&quot;api/[controller]&quot;)]
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
            System.Console.WriteLine(&quot;bombing out ...&quot;);
            //purposely crash
            System.Environment.Exit(100);
            return null;
        }
    }
}
</code></pre>
</li>
<li>
<p>Deploy the updated service to Cloud Foundry.</p>
</li>
<li>
<p>Confirm that &quot;regular&quot; URL still works, by navigating to the
<code>/api/products</code> endpoint of your microservice.</p>
</li>
<li>
<p>Now send a request to the <code>/api/failure</code> endpoint.</p>
</li>
<li>
<p>See in Applications Manager that the app crashes, and the platform
quickly recovers and spins up a new instance.</p>
</li>
<li>
<p>Visit the &quot;logs&quot; view and see that logs were written out.</p>
</li>
</ol>
