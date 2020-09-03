using System;

namespace Atom.Api.Infrastructure.Persistence.Interfaces
{
    public interface IFileHandler
    {

        bool FileExists(string fullPathWithFilename);
        Byte[] GetResizedImage(int width, int height);
    }
}
