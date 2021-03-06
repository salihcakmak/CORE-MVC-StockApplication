﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CORE_MVC_STOK.Data.Context;
using CORE_MVC_STOK.DataAccess.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CORE_MVC_STOK
{
    public class Startup
    {
        //appsetting.json dosyamızı okumamızı sağlayan yapıcı metod.
        //Dependency Injection için
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //appsetting.json dosyamızı okumamızı sağlayan değişken tanımlaması
        //Dependency Injection için
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var a = $"server={Environment.GetEnvironmentVariable("server")};database={Environment.GetEnvironmentVariable("database")};user={Environment.GetEnvironmentVariable("user")};password={Environment.GetEnvironmentVariable("password")}";


            //Bağlantıyı projeye ekleyen kod.Contextimiz ile veri tabanını bağlar.
            services.AddDbContext<MasterContext>(options => options.UseMySQL($"server={Environment.GetEnvironmentVariable("server")};database={Environment.GetEnvironmentVariable("database")};user={Environment.GetEnvironmentVariable("user")};password={Environment.GetEnvironmentVariable("password")}"));



            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
        
           
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                //Uygulamanın kullandığı DatabaseContext sınıfından bir örnek alıyoruz.
                var context = serviceScope.ServiceProvider.GetRequiredService<MasterContext>();
                //DbContext'i migrate ediyoruz.
                context.Database.Migrate();
            }
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
