using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HttpClientFactoryTesting.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttpClientFactoryTesting
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddHttpClient("Name", config => {
               config.BaseAddress = new Uri("http://www.google.com");
            });

            services.AddHttpClient<SomeService>();

            services.AddHttpClient<SomeService>().AddHttpMessageHandler<RetryHandler>();
            // what this is effectively middleware for the outogoing request
            // if we run this the retry handler will be called first before the actual handler which will send the request.
            // you could wrap the sendAsync and catch an exception to attempt a retry for example

            services.AddMvc();
        }

        public class RetryHandler : DelegatingHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
            {
                return base.SendAsync(requestMessage, cancellationToken);
            }   
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
