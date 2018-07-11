<h1 id="introduction">Introduction</h1>
<p>In this exercise, we work with a Git-backed config store and build an app that
consumes it.</p>

<h1 id="point-config-server-to-git-repository">Point Config Server to Git repository</h1>

<ol>
<li>
<p>Create a file called <code>config.json</code> with the following contents. We
will use this file to tell the Config Server where to get its
configurations from.</p>
  
```json
{
  "git": {
    "uri": "https://github.com/cdevpivotal/sc-workshop", "label": "config"
  }
}
```

</li>
<li>
<p>From the Terminal within Visual Studio Code, enter the following
command.</p>
  
```shell
cf update-service <your config server name> -c config.json
```
  
</li>
<li>
<p>From Apps Manager, navigate to the &quot;Services&quot; view, manage the Config
Server instance, and notice the git URL in the configuration.</p>
</li>
</ol>

<h1 id="create-aspnet-core-web-api">Create ASP.NET Core Web API</h1>

<p>Let's create a brand new microservice and set it up with the Steeltoe
libraries that pull configuration values from our Spring Cloud Services
Config Server.</p>
<ol>
<li>
<p>In the Visual Studio Code Terminal window, navigate up one level and
create a new folder (bootcamp-webapi) to hold our data microservice.</p>
</li>
<li>
<p>From within that folder, run the
<code>dotnet new webapi</code> command in the
Terminal. This uses the &quot;web api&quot; template to create scaffolding for a
web service.</p>
</li>
<li>
<p>From the Terminal enter</p>

```shell
dotnet add package Microsoft.Extensions.Options
dotnet add package Microsoft.Extensions.Configuration
dotnet add package Pivotal.Extensions.Configuration.ConfigServerCore --version 2.0.0
```
</li>
<li>
<p>Open the newly created folder in Visual Studio Code.</p>
</li>
<li>
<p>In the Controllers folder, create a new file named
<code>ProductsController.cs</code>.</p>
</li>
<li>
<p>Enter the following code. It defines a new API controller, specifies
the route that handles it and an operation we can invoke.</p>

```C#
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
```

</li>
</ol>

<h1 id="connect-application-to-config-server">Connect application to Config Server</h1>

<p>Next, we add what's needed to make our ASP.NET Core application
retrieve its configuration from Cloud Foundry and the Config Server.</p>
<ol>
<li>
<p>Edit <code>appsettings.json</code> to include the application name and cloud
config name. The cloud config name maps to a configuration file read from the server, here this is branch 1. 
By explicity setting the config to this file (branch1), we ensure environment variables won't overwrite configuration.</p>

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
 "spring": {
   "application": {
     "name": "core-cf-microservice-<enter your name>"
   },
   "cloud": {
     "config": {
       "name": "branch1"
     }
   }
 }
}
```

</li>
<li>
<p>In <code>Program.cs</code>, set up configuration providers in the following
order:</p>
<ol>
<li>Environment variables</li>
<li><code>appsettings.json</code></li>
<li>Environment-specific <code>appsettings.json</code></li>
<li>Our config store</li>
</ol>
  
```C#
// ...
using Pivotal.Extensions.Configuration.ConfigServer;

namespace bootcamp_webapi
{
    public class Program
    {
        // ...

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
               .ConfigureAppConfiguration((hostContext, config) =>
               {
                   var env = hostContext.HostingEnvironment;
                   config.Sources.Clear();
                   config.AddEnvironmentVariables();
                   config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                   config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                   config.AddConfigServer(env);
               });
    }
}
```

</li>
<li>
<p>In <code>Startup.cs</code>, add configuration to the service container, allowing
it to be injected into the <code>ProductsController</code>.</p>

```C#
// ...
using Pivotal.Extensions.Configuration.ConfigServer;

  namespace bootcamp_webapi
  {
      public class Startup
      {
          // ...

          public void ConfigureServices(IServiceCollection services)
          {
             services.AddMvc();
             services.AddConfiguration(Configuration);
          }

          // ...
      }
  }
```

</li>
<li>
<p>Add a <code>manifest.yml</code> file to the base directory of the project. This tells
Cloud Foundry how to deploy your app. Enter:</p>

```yaml
---
applications:
- name: core-cf-microservice-<enter your name>
  instances: 1
  memory: 256M
  # determines which environment to pull configs from
  env:
    ASPNETCORE_ENVIRONMENT: dev
  services:
    - <your config server instance name>
```

</li>
<li>
<p>Execute <code>cf push</code> to deploy this application to Cloud Foundry! Note
the <strong>route</strong> of your microservice:</p>

```
Waiting for app to start...

name:              core-cf-yourname
requested state:   started
instances:         1/1
usage:             256M x 1 instances
routes:            core-cf-yourname.apps.chicken.pal.pivotal.io
last uploaded:     Wed 28 Mar 09:19:42 MDT 2018
stack:             cflinuxfs2
buildpack:         https://github.com/cloudfoundry/dotnet-core-buildpack#v2.0.5
start command:     cd ${DEPS_DIR}/0/dotnet_publish && ./mvctest --server.urls http://0.0.0.0:${PORT}
```

</li>
</ol>

<h1 id="observe-behavior-when-changing-application-name-or-label">Observe behavior when changing application name or label</h1>

<p>See how configurations are pulled and used.</p>
<ol>
<li>
<p>Navigate to the <code>/api/products</code> endpoint of your microservice and
observe the result.</p>
</li>
<li>
<p>Go to the "Logs"; view in Apps Manager and see the connection string
and log level written out.</p>
</li>
<li>
<p>Go back to the source code and change the application name and cloud
config name in <code>appsettings.json</code> to "branch3", and in 
<code>manifest.yml</code> change the environment to "qa." This should resolve to a
different configuration file in the GitHub repo, and load different
values into the app's configuration properties.</p>
</li>
<li>
<p>Re-deploy the app (<code>cf push</code>), hit the API endpoint, and observe the
different values logged out.</p>
</li>
</ol>
