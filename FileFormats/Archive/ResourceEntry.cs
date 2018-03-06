
namespace FileFormats.Archive
{
    public class ResourceEntry
    {
        public uint TypeId;
        public ushort Version;
        public byte[] Data;
        public uint SlotRamRequired;
        public uint SlotVramRequired;
        public uint OtherRamRequired;
        public uint OtherVramRequired;
        public uint Unknown1; // M3
        public uint Unknown2; // M3
    }
}
