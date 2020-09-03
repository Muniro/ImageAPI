using Atom.Api.Application.Features.Products.Queries.GetImage;
using Atom.Api.Infrastructure.Shared.Common_Interfaces;
using Atom.Api.WebApi.Configurations;
using Atom.Api.WebApi.Extensions;
using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Atom.Api.WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ImageController : BaseApiController
    {
        private string path;
        private readonly string folderName;
        private readonly IRedisCacheService redisCacheService;

        public ImageController(IOptions<ImageOptions> options, IHostEnvironment envProvider, IRedisCacheService redisCacheService)
        {
            this.path=  envProvider.ContentRootPath;
            this. folderName = options.Value.FolderName;
            this.redisCacheService = redisCacheService;


        }

        // GET: api/<controller>
        [HttpGet]
        [Route("Get/{name}/{width}/{height}/{type}/{watermark?}/{backgroundColor?}")]
        public async Task<IActionResult> Get (string name,int width, int height, string type, string watermark="none", string backgroundColor="none" )
        {

            //check distributed cache (Redis) if we had this exact request previously and we have it in cache!

            string concatenatedKey = string.Concat(name, width, height, type, watermark, backgroundColor);

            FileContentResult fcr = null;
            string key = HashExtensions.GetMD5HashString(concatenatedKey);
           
            if (redisCacheService.HasValudBeenCached(key).Result)
            {
                //we have this in cache!
                //string valueInCache= redisCacheService.GetCachedValueAsync(key).Result;

                byte[] imageBytes = redisCacheService.GetCachedValueAsync(key).Result; //System.Text.Encoding.UTF8.GetBytes(valueInCache);
                fcr = File(imageBytes, $"image/{type}");
            }
            else
            {

                var query = new GetImageQuery()
                {
                    Name = name,
                    Width = width,
                    Height = height,
                    ImageType = type,
                    Watermark = watermark,
                    BackgroundColor = backgroundColor,
                    ImageFileNamePath = Path.Combine(path, folderName, name)

                };

                var result = await Mediator.Send(query);
                fcr= File(result.Data.ImageByteArray, $"image/{type}");

                //Cache it now
                redisCacheService.SetCacheValueAsync(key, result.Data.ImageByteArray);
            }


            return fcr;
        }
    }
}
