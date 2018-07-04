 <h1 id="introduction">Introduction</h1>
<p>This exercise helps us understand how to register our microservices with the Spring
Cloud Services Registry, and also discover those services at runtime.</p>
<h3 id="update-previous-service-to-register-itself-with-the-service-registry">Update previous service to register itself with the Service Registry</h3>
<p>Modify our products microservice to automatically register itself upon
startup.</p>
<ol>
<li>
<p>In the existing microservice project, add a Nuget package dependency
by entering the following command into the Terminal.</p>
<pre><code class="language-bash">dotnet add package Pivotal.Discovery.ClientCore --version 2.0.0
</code></pre>
<p><em>If you weren’t using Spring Cloud Services, you could use a vanilla
Steeltoe package for service discovery:
<code>Steeltoe.Discovery.ClientCore</code>.</em></p>
</li>
<li>
<p>Open the <code>appsettings.json</code> file and add this block after the
&quot;spring&quot; block:</p>
<pre><code class="language-diff">{
  &quot;Logging&quot;: {
    &quot;IncludeScopes&quot;: false,
    &quot;LogLevel&quot;: {
      &quot;Default&quot;: &quot;Warning&quot;
    }
  },
  &quot;spring&quot;: {
    &quot;application&quot;: {
      &quot;name&quot;: &quot;branch1&quot;
    },
    &quot;cloud&quot;: {
      &quot;config&quot;: {
        &quot;name&quot;: &quot;branch1&quot;
      }
    }
  },
+ &quot;eureka&quot;: {
+   &quot;client&quot;: {
+     &quot;shouldRegisterWithEureka&quot;: true,
+     &quot;shouldFetchRegistry&quot;: true
+   }
+ }
}
</code></pre>
</li>
<li>
<p>In <code>Startup.cs</code>, add the discovery client to the service container.</p>
<pre><code class="language-diff">  // ...
+ using Pivotal.Discovery.Client;


namespace bootcamp_webapi
{
    public class Startup
    {
        // ...

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddConfiguration(Configuration);
+           services.AddDiscoveryClient(Configuration);
        }

        // ...
    }
}    
</code></pre>
</li>
<li>
<p>In the same class, update the Configure method to use the discovery
client.</p>
<pre><code class="language-diff">public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    if (env.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }

    app.UseMvc();
+   app.UseDiscoveryClient();
}
</code></pre>
</li>
<li>
<p>Update manifest so that the microservice also binds to the Service
Registry upon push.</p>
<pre><code class="language-diff">  ---
  applications:
  - name: core-cf-microservice-&lt;enter your application name&gt;
    instances: 1
    memory: 256M
    # determines which environment to pull configs from
    env:
      ASPNETCORE_ENVIRONMENT: qa
    services:
      - &lt;your configuration service name&gt;
+     - &lt;your registry name&gt;
</code></pre>
</li>
<li>
<p>Push the application (<code>cf push</code>). Go &quot;manage&quot; the Service Registry
instance from within Apps Manager. Notice our service is now listed!</p>
</li>
</ol>
<h1 id="download-and-open-up-front-end-aspnet-core-application">Download and open up front-end ASP.NET Core application</h1>
<p>Configure a pre-built front end app so that it discovers our products
microservice.</p>
<ol>
<li>
<p>Go to <a href="https://github.com/cdevpivotal/dotnet-workshop/tree/master/cloud-native-net-sampleui" rel="noreferrer noopener">https://github.com/cdevpivotal/dotnet-workshop/tree/master/cloud-native-net-sampleui</a>
to download the pre-built front-end application.</p>
</li>
<li>
<p>Download the code into a folder on your machine.</p>
</li>
<li>
<p>Open the project in Visual Studio Code.</p>
</li>
<li>
<p>Observe in <code>bootcamp-core-ui.csproj</code> that the project references the
discovery package. See in <code>appsettings.json</code> that this app does <strong>NOT</strong>
register itself with Eureka, but just pulls the registry. Also see in
the <code>Startup.cs</code> file that it loads the discovery service.</p>
</li>
</ol>
<h1 id="update-front-end-application-to-pull-services-from-the-registry-and-use-them">Update front-end application to pull services from the registry and use them</h1>
<p>Replace placeholder values so that your app talks to your own
microservice.</p>
<ol>
<li>
<p>Open the <code>HomeController.cs</code> file in the Controllers folder.</p>
</li>
<li>
<p>Go to the <code>Index()</code> operation and replace the placeholder string
with the microservice’s application name (<strong>not the url</strong>) from your
Service Registry.</p>
<pre><code class="language-diff">[Route(&quot;/home&quot;)]
public IActionResult Index() {
    var client = GetClient();
+   var result = client.GetStringAsync(&quot;https://&lt;replace me&gt;/api/products&quot;).Result;
    ViewData[&quot;products&quot;] = result;
    return View();
}
</code></pre>
</li>
<li>
<p>In <code>manifest.yml</code>, replace the &quot;application name&quot; and &quot;service
registry name&quot; placeholders.</p>
</li>
<li>
<p>Push your application to Cloud Foundry (<code>cf push</code>).</p>
</li>
<li>
<p>Go to the <code>/Home</code> endpoint of your application. You should see the
web page with products retrieved from your data microservices.</p>
</li>
</ol>
