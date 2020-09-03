using Atom.Api.Application;
using Atom.Api.Application.Interfaces;
using Atom.Api.Application.Utilities;
using Atom.Api.Infrastructure.Persistence;
using Atom.Api.Infrastructure.Shared;
using Atom.Api.Infrastructure.Shared.Common_Interfaces;
using Atom.Api.WebApi.Configurations;
using Atom.Api.WebApi.Extensions;
using Atom.Api.WebApi.Services;
using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Atom.Api.WebApi
{
    public class Startup
    {

        const string cacheMaxAge = "604800";
        public IConfiguration _config { get; }
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationLayer();
            var imageOptionsSection =
                _config.GetSection("ImageOptions");
            services.Configure<ImageOptions>(imageOptionsSection);

            services.AddPersistenceInfrastructure(_config);
            services.AddSharedInfrastructure(_config);
            services.AddSwaggerExtension();
            services.AddControllers();
            services.AddApiVersioningExtension();
            services.AddHealthChecks();
            
            services.AddEasyCaching(options =>
            {
                //use redis cache
                options.UseRedis(redisConfig =>
                {
                    //Setup Endpoint, could be dynamically set the server and port details.
                    redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));

                    /* For production recommend setting up Password!, like this */
                    //Setup password if applicable
                    //if (!string.IsNullOrEmpty(serverPassword))
                    //{
                    //    redisConfig.DBConfig.Password = serverPassword;
                    //}

                    /* ======================= */


                    //Allow admin operations
                    redisConfig.DBConfig.AllowAdmin = true;
                },
                    "LocalRedis");
            });
            services.AddTransient<IFileHandler, FileHandler>();
            services.AddTransient<IRedisCacheService, RedisCacheService>();
           
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "Product_Images")),
                RequestPath = "/Product_Images",
                OnPrepareResponse = ctx =>
                {
                    // sets the Cache-Control heade
                    ctx.Context.Response.Headers.Add(
                         "Cache-Control", $"public, max-age={cacheMaxAge}");
                }
            }) ;

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerExtension();
            app.UseErrorHandlingMiddleware();
            app.UseHealthChecks("/health");

            app.UseEndpoints(endpoints =>
             {
                 endpoints.MapDefaultControllerRoute();
             });
        }
    }
}
