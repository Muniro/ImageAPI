using System;
using System.Collections.Generic;
using System.Text;

namespace Atom.Api.Application.Filters
{
    public class RequestParameter
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string ImageType { get; set; }

        public string WaterMark { get; set; }

        public string BackgroundColor { get; set; }
        public RequestParameter()
        {
            this.Width = 400;
            this.Height = 400;
            this.ImageType = "png";
            WaterMark = String.Empty;
            BackgroundColor = String.Empty;
        }
        public RequestParameter(int width, int height, string imageType, string waterMark, string backgroundColor)
        {
            this.Width = width;
            this.Height = height;
            this.ImageType = imageType;

            this.WaterMark = waterMark;
            this.BackgroundColor = backgroundColor;
            
        }







    }
}
