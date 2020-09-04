
using Atom.Api.Application.Interfaces;
using LazZiya.ImageResize;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace Atom.Api.Application.Utilities
{
    public class FileHandler : IFileHandler
    {
        public byte[]  GetResizedImage(string imagePathAndName, int width, int height, string waterMark="", string backgroundColor="")
        {

            byte[] resizeImageByte;
            using (Image img = Image.FromFile(imagePathAndName))
            {

                var scaledImage = img.Scale(width, height, new GraphicOptions() { 
                PageUnit=GraphicsUnit.Pixel
                });
                var tOps = new TextWatermarkOptions();
                tOps = ApplyWaterMark(waterMark, backgroundColor, scaledImage, tOps);

                resizeImageByte = GetImageByteArray(scaledImage);

            }

            return resizeImageByte; 
        }

        private byte[] GetImageByteArray(Image scaledImage)
        {
            byte[] resizeImageByte;
            using (var stream = new MemoryStream())
            {
                scaledImage.Save(stream, ImageFormat.Png);
                resizeImageByte = stream.ToArray();

            }

            return resizeImageByte;
        }

        private TextWatermarkOptions ApplyWaterMark(string waterMark, string backgroundColor, Image scaledImage, TextWatermarkOptions tOps)
        {
           
            if (!string.IsNullOrEmpty(backgroundColor))
            {
                tOps = new TextWatermarkOptions
                {
                    BGColor = Color.FromName(backgroundColor)
                };

            }

            if (!string.IsNullOrEmpty(waterMark))
            {
                scaledImage.AddTextWatermark(waterMark, tOps);

            }

            return tOps;
        }
    }
}
