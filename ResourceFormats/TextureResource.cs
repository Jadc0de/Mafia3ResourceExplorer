using System;
using System.IO;
using Gibbed.IO;

namespace ResourceFormats
{
    public class TextureResource : IResourceType
    {
        public ulong NameHash;
        public byte MipMap;
        public byte[] Data;

        public void Serialize(ushort version, Stream output, Endian endian)
        {
            output.WriteValueU64(NameHash, endian);
            output.WriteValueU8(MipMap);
            output.WriteBytes(Data);
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            NameHash = input.ReadValueU64(); // "dlc1_uni_dirt_v1---d.dds"
            MipMap = input.ReadValueU8();  // always == 0 version == 3
            Data = new byte[input.Length - input.Position];
            input.Read(Data, 0, Data.Length);
        }
    }
}
