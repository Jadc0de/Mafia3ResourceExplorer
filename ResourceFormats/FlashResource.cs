using System;
using System.IO;
using FileFormats;
using Gibbed.IO;

namespace ResourceFormats
{
    public class FlashResource : IResourceType
    {
        public string Path;
        public string Short;
        public byte[] Data;

        public void Serialize(ushort version, Stream output, Endian endian)
        {
            output.WriteStringU16(Path, endian);
            output.WriteValueU64(FileFormats.Hashing.FNV64.Hash(Short), endian);
            output.WriteStringU16(Short, endian);
            output.WriteValueU32((uint)Data.Length, endian);
            output.WriteBytes(Data);
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            Path = input.ReadStringU16(endian);
            input.ReadValueU64(endian); // Hash
            Short = input.ReadStringU16(endian);
            uint size = input.ReadValueU32(endian);
            Data = input.ReadBytes(size);
        }

    }
}
