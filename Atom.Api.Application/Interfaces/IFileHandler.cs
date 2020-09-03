using System;
using System.Threading.Tasks;

namespace Atom.Api.Application.Interfaces
{
    public interface IFileHandler
    {

        public byte[] GetResizedImage(string imagePathAndName, int width, int height ,string waterMark = "", string backgroundColor = "");
    }
}
