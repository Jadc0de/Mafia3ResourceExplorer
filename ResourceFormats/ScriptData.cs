using System.IO;
using Gibbed.IO;

namespace ResourceFormats
{
    public class ScriptData : IResourceType
    {
        public ulong NameHash;
        public ulong DataHash;
        public string Name;
        public byte[] Data;

        public void Serialize(ushort version, Stream output, Endian endian)
        {
            // Mafia II  = 2
            // Mafia III = 4
            if (version == 4)
            {
                NameHash = FileFormats.Hashing.FNV64.Hash(Name);
                DataHash = FileFormats.Hashing.FNV64.Hash(Data, 0, Data.Length);
                output.WriteValueU64(NameHash, endian);
                output.WriteValueU64(DataHash, endian);
            }

            output.WriteStringU16(Name, endian);
            output.WriteValueS32(Data.Length, endian);
            output.WriteBytes(Data);
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            // Mafia II  = 2
            // Mafia III = 4
            if (version == 4)
            {
                NameHash = input.ReadValueU64(endian);
                DataHash = input.ReadValueU64(endian);
            }

            Name = input.ReadStringU16(endian);
            var size = input.ReadValueU32(endian);
            Data = input.ReadBytes(size);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
