using System.IO;
using Gibbed.IO;

namespace FileFormats.Archive
{
    internal struct ResourceHeader
    {
        public uint TypeId;
        public uint Size; // includes this header
        public ushort Version;
        public uint SlotRamRequired;
        public uint SlotVramRequired;
        public uint OtherRamRequired;
        public uint OtherVramRequired;
        // public ushort _f1E;
        // public uint _f20;
        public uint Unknown1;
        public uint Unknown2;

        public static ResourceHeader Read(Stream input, Endian endian)
        {
            ResourceHeader instance;
            instance.TypeId = input.ReadValueU32(endian);
            instance.Size = input.ReadValueU32(endian);
            instance.Version = input.ReadValueU16(endian);
            instance.SlotRamRequired = input.ReadValueU32(endian);
            instance.SlotVramRequired = input.ReadValueU32(endian);
            instance.OtherRamRequired = input.ReadValueU32(endian);
            instance.OtherVramRequired = input.ReadValueU32(endian);

            instance.Unknown1 = input.ReadValueU32(endian); // M3
            instance.Unknown2 = input.ReadValueU32(endian); // M3

            return instance;
        }

        public void Write(Stream output, Endian endian)
        {
            output.WriteValueU32(TypeId, endian);
            output.WriteValueU32(Size, endian);
            output.WriteValueU16(Version, endian);
            output.WriteValueU32(SlotRamRequired, endian);
            output.WriteValueU32(SlotVramRequired, endian);
            output.WriteValueU32(OtherRamRequired, endian);
            output.WriteValueU32(OtherVramRequired, endian);

            output.WriteValueU32(Unknown1, endian); // M3
            output.WriteValueU32(Unknown2, endian); // M3
        }
    }
}
