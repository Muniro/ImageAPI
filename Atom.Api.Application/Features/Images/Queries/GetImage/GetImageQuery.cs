using Atom.Api.Application.Features.Images.Queries.GetImage;
using Atom.Api.Application.Wrappers;
using MediatR;

namespace Atom.Api.Application.Features.Products.Queries.GetImage
{
    public class GetImageQuery : IRequest<Response<ImageViewModel>>
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string ImageType { get; set; }

        public string Watermark { get; set; }
        public string BackgroundColor { get; set; }

        public string ImageFileNamePath { get; set; }
    }
   
}
