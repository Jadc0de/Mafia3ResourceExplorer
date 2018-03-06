using System;
using System.IO;
using FileFormats;
using Gibbed.IO;

namespace ResourceFormats
{
    public class FlashResource : IResourceType
    {
        public string Name; // Path
        //public string Short;
        public byte[] Data;

        public void Serialize(ushort version, Stream output, Endian endian)
        {
            string Short = Path.GetFileNameWithoutExtension(Name);
            output.WriteStringU16(Name, endian);
            //output.WriteValueU64(_Hash, endian);
            output.WriteValueU64(FileFormats.Hashing.FNV64.Hash(Short), endian);
            output.WriteStringU16(Short, endian);
            output.WriteValueU32((uint)Data.Length, endian);
            output.WriteBytes(Data);
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            Name = input.ReadStringU16(endian);
            input.ReadValueU64(endian); // Hash
            //Short = input.ReadStringU16(endian);
            input.ReadStringU16(endian);
            uint size = input.ReadValueU32(endian);
            Data = input.ReadBytes(size);

#if _DEBUG
            // weapon_icons_inworld_i71.dds
            // vehicle_delivery_i2f.dds
            /*string buf = null;
            for (int i = 1; i < 256; i++)
            {
                buf += string.Format("hud_vehicle_i{0:x}.dds\r\n", i);
            }*/

            string sh = Path.GetFileNameWithoutExtension(Name);
            if (sh != Short)
            {
                System.Diagnostics.Debugger.Break();
            }
#endif
        }

    }
}
