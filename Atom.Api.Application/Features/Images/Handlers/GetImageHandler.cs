using Atom.Api.Application.Features.Images.Queries.GetImage;
using Atom.Api.Application.Features.Products.Queries.GetImage;
using Atom.Api.Application.Interfaces;
using Atom.Api.Application.Wrappers;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using LazZiya.ImageResize;

namespace Atom.Api.Application.Features.Images.Handlers
{
    public class GetImageHandler : IRequestHandler<GetImageQuery, Response<ImageViewModel>>
    {
        private readonly IFileHandler _fileHandler;

        public GetImageHandler(IFileHandler fileHandler)
        {
            _fileHandler = fileHandler;
        }

        public Task<Response<ImageViewModel>> Handle(GetImageQuery request, CancellationToken cancellationToken)
        {

            var newImageSize = _fileHandler.GetResizedImage(request.ImageFileNamePath, request.Width, request.Height, 
                request.Watermark, request.BackgroundColor);

            var ret = new ImageViewModel()
            {
                ImageByteArray = newImageSize
            };

            var ret2 = new Response<ImageViewModel>()
            {
                Data = ret,
                Succeeded = true,
                Message = "Successfully resized"
            };

            return Task.FromResult(ret2);
        }
    }
}
