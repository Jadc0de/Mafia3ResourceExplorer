using System;
using System.IO;
using FileFormats;
using Gibbed.IO;

namespace ResourceFormats
{
    public class MemFileResource : IResourceType
    {
        public uint Unk0;
        public string Name;
        public uint Unk1;
        public uint UType;
        public byte[] Data;

        public void Serialize(ushort version, Stream output, Endian endian)
        {
            output.WriteValueU32(Unk0, endian);
            output.WriteStringU32(Name, endian);
            output.WriteValueU32(Unk1, endian);
            output.WriteValueU32(UType, endian);
            output.WriteValueS32(Data.Length, endian);
            output.WriteBytes(Data);
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            Unk0 = input.ReadValueU32(endian);  // 0 or Hash?
            Name = input.ReadStringU32(endian); // xml == this
            Unk1 = input.ReadValueU32(endian);
            if (Unk1 != 1)
            {
                throw new InvalidOperationException();
            }

            UType = input.ReadValueU32(); // M3 Type 4 text, 16 bink

#if DEBUG
            if (UType != 4 && UType != 16)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif

            uint size = input.ReadValueU32(endian);
            Data = input.ReadBytes(size);
        }

    }
}
