using System.IO;
using Gibbed.IO;

namespace ResourceFormats
{
    public interface IResourceType
    {
        void Serialize(ushort version, Stream output, Endian endian);
        void Deserialize(ushort version, Stream input, Endian endian);
    }
}
