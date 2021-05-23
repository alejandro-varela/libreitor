using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Mvc.Authorization;
using Basics.Controllers;
using Basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Basics.CustomPolicyProvider;

namespace Basics
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config =>
            {
                config.Cookie.Name  = "Grandmas.Cookie";
                config.LoginPath    = "/Home/Authenticate";
            });

            services.AddAuthorization(config =>
            {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultPolicy = defaultAuthBuilder
                //    .RequireAuthenticatedUser()
                //    .RequireClaim(ClaimTypes.DateOfBirth)
                //    .Build();

                config.AddPolicy("Admin", policyBuilder => {
                    policyBuilder.RequireClaim(ClaimTypes.Role, "Admin");
                });

                config.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                });
            });

            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();

            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            services.AddControllersWithViews(config => {
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultPolicy = defaultAuthBuilder
                //    .RequireAuthenticatedUser()
                //    .Build();
                //config.Filters.Add(new AuthorizeFilter());
            });

            services
                .AddRazorPages()
                .AddRazorPagesOptions(config => {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                    config.Conventions.AuthorizePage("/Razor/Policy", "Admin");
                    config.Conventions.AuthorizeFolder("/RazorSecured");
                    config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            ////// SIEMPRE DESPUES DE ROUTING //////
            // ¿quien sos?
            app.UseAuthentication();
            // ¿estas autorizado?
            app.UseAuthorization();
            ////////////////////////////////////////

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context =>
                //{
                //    await context.Response.WriteAsync("Hello World!");
                //});
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
