using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", config => // Microsoft.AspNetCore.Authentication.JwtBearer
                {
                    var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
                    var key = new SymmetricSecurityKey(secretBytes);

                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer      = Constants.Issuer,
                        ValidAudience    = Constants.Audience,
                        IssuerSigningKey = key,
                    };

                    config.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = (context) => 
                        {
                            const string nombreParamToken = "access_token";
                            if (context.Request.Query.ContainsKey(nombreParamToken))
                            {
                                context.Token = context.Request.Query[nombreParamToken];
                            }
                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddControllersWithViews();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
