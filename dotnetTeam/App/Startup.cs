using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using App.EventStore;
using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

	        services.AddTransient<IEventsPublisher, EventsPublisher>();
//	        services.AddTransient<IEventStore, EventStore.EventStoreFileSystem>(provider => new EventStore.EventStoreFileSystem(Environment.GetEnvironmentVariable("WORKSHOP_EVENTSTORE_DIRECTORY") ?? Path.Combine(Directory.GetCurrentDirectory(), "DATA")));
	        services.AddTransient<IEventStore, EventStoreDb>(provider => 
                new EventStoreDb(
                    Environment.GetEnvironmentVariable("WORKSHOP_EVENTSTORE_HOST") ?? "127.0.0.1",
                    Environment.GetEnvironmentVariable("WORKSHOP_EVENTSTORE_LOGIN") ?? "admin",
                    Environment.GetEnvironmentVariable("WORKSHOP_EVENTSTORE_PASSWORD") ?? "changeit"));
	        services.AddTransient<IRoomRepository, RoomRepository>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

//            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}
