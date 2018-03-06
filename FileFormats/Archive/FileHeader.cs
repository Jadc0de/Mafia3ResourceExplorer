using System.IO;
using Gibbed.IO;

namespace FileFormats.Archive
{
    public struct FileHeader
    {
        public uint ResourceTypeTableOffset;
        public uint BlockTableOffset;
        public uint XmlOffset;
        public uint SlotRamRequired;
        public uint SlotVramRequired;
        public uint OtherRamRequired;
        public uint OtherVramRequired;
        public uint Flags;
        public byte[] Unknown20;
        public uint ResourceCount;

        public void Write(Stream output, Endian endian)
        {
            output.WriteValueU32(ResourceTypeTableOffset, endian);
            output.WriteValueU32(BlockTableOffset, endian);
            output.WriteValueU32(XmlOffset, endian);
            output.WriteValueU32(SlotRamRequired, endian);
            output.WriteValueU32(SlotVramRequired, endian);
            output.WriteValueU32(OtherRamRequired, endian);
            output.WriteValueU32(OtherVramRequired, endian);
            output.WriteValueU32(Flags, endian);
            output.Write(Unknown20, 0, Unknown20.Length);
            output.WriteValueU32(ResourceCount, endian);
        }

        public static FileHeader Read(Stream input, Endian endian)
        {
            FileHeader instance;
            instance.ResourceTypeTableOffset = input.ReadValueU32(endian);
            instance.BlockTableOffset = input.ReadValueU32(endian);
            instance.XmlOffset = input.ReadValueU32(endian);
            instance.SlotRamRequired = input.ReadValueU32(endian);
            instance.SlotVramRequired = input.ReadValueU32(endian); // VRam
            instance.OtherRamRequired = input.ReadValueU32(endian); // Ram
            instance.OtherVramRequired = input.ReadValueU32(endian);
            instance.Flags = input.ReadValueU32(endian);
            instance.Unknown20 = input.ReadBytes(16);
            instance.ResourceCount = input.ReadValueU32(endian);
            return instance;
        }
    }
}
