using Atom.Api.Application.Features.Products.Queries.GetImage;
using Atom.Api.Infrastructure.Shared.Common_Interfaces;
using Atom.Api.WebApi.Configurations;
using Atom.Api.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Atom.Api.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ImageController : BaseApiController
    {
        private string path;
        private readonly string folderName;
        private readonly IRedisCacheService redisCacheService;

        public ImageController(IOptions<ImageOptions> imageOptions, IHostEnvironment envProvider, IRedisCacheService redisCacheService)
        {
            this.path=  envProvider.ContentRootPath;
            this. folderName = imageOptions.Value.FolderName;
            this.redisCacheService = redisCacheService;
        }

        // GET: api/<controller>
        [HttpGet]
        [ResponseCache(Duration = 1800)]
        [Route("Get/{name}/{width}/{height}/{type}/{watermark?}/{backgroundColor?}")]
        public async Task<IActionResult> Get (string name,int width, int height, string type, string watermark="", string backgroundColor="")
        {
            // Get the requested ETag
            string requestETag = "";
            if (Request.Headers.ContainsKey("If-None-Match"))
            {
                requestETag = Request.Headers["If-None-Match"].First();
            }

            //check distributed cache (Redis) if we had this exact request previously and we have it in cache!
            string concatenatedKey = GetConcatenatedKey(name, width, height, type, watermark, backgroundColor);

            FileContentResult fcr = null;
            string key = GenerateHashKey(concatenatedKey);
            byte[] imageBytes;
            if (redisCacheService.HasValudBeenCached(key).Result)
            {
                //we have this in cache!
                imageBytes = redisCacheService.GetCachedValueAsync(key).Result;
               
            }
            else
            {
                GetImageQuery query = GetImageQueryObject(name, width, height, type, watermark, backgroundColor);

                //Using CQRS technique to send the query to the mediatR

                var result = await Mediator.Send(query);

                imageBytes = result.Data.ImageByteArray;         
                //Cache it now
                redisCacheService.SetCacheValueAsync(key, imageBytes);
            }

           

            // Construct the new ETag
            string responseETag = key;

            // Return a 304 if the ETag of the current record matches the ETag in the "If-None-Match" HTTP header
            if (Request.Headers.ContainsKey("If-None-Match") && responseETag == requestETag)
            {
                return Ok(StatusCode((int)HttpStatusCode.NotModified));
            }

            // Add the current ETag to the HTTP header
            Response.Headers.Add("ETag", responseETag);

            fcr = GetImageFile(type, imageBytes);
            return fcr;
        }

        private void CheckEtagAndAddToHeaders(string key)
        {
            throw new NotImplementedException();
        }

        private FileContentResult GetImageFile(string type, byte[] imageBytes)
        {
            return File(imageBytes, $"image/{type}");
        }

        private GetImageQuery GetImageQueryObject(string name, int width, int height, string type, string watermark, string backgroundColor)
        {
            return new GetImageQuery()
            {
                Name = name,
                Width = width,
                Height = height,
                ImageType = type,
                Watermark = watermark,
                BackgroundColor = backgroundColor,
                ImageFileNamePath = Path.Combine(path, folderName, name)

            };
        }

        private string GetConcatenatedKey(string name, int width, int height, string type, string watermark, string backgroundColor)
        {
            return string.Concat(name, width, height, type, watermark, backgroundColor);
        }

        private string GenerateHashKey(string concatenatedKey)
        {
           /* We hash the parameters together to form a unique hash for use as Redis cache Key, 
            * that represents uniquely that combination and we use MD5 hash */
            return HashExtensions.GetMD5HashString(concatenatedKey);
        }
    }
}
