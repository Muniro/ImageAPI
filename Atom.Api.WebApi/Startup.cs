using Atom.Api.Application;
using Atom.Api.Application.Interfaces;
using Atom.Api.Application.Utilities;
using Atom.Api.Infrastructure.Persistence;
using Atom.Api.Infrastructure.Persistence.Cache;
using Atom.Api.Infrastructure.Shared;
using Atom.Api.Infrastructure.Shared.Common_Interfaces;
using Atom.Api.WebApi.Configurations;
using Atom.Api.WebApi.Extensions;
using EasyCaching.Core.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.IO.Compression;
using System.Linq;

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

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            services.AddResponseCompression(options =>
            {
                  options.Providers.Add<GzipCompressionProvider>();
                  options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                     "image/png"});
            });


            services.AddPersistenceInfrastructure(_config);
            services.AddSharedInfrastructure(_config);
            services.AddSwaggerExtension();
            services.AddControllers();
            services.AddApiVersioningExtension();
            services.AddHealthChecks();
            services.AddResponseCaching();
            services.AddTransient<IFileHandler, FileHandler>();
                      
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
           //not used at all but would recommend! app.UseHttpsRedirection();

            app.UseResponseCompression();
            app.UseResponseCaching();
            
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "Product_Images")),
                RequestPath = "/Product_Images",
                OnPrepareResponse = ctx =>
                {
                    // sets the Cache-Control header
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
