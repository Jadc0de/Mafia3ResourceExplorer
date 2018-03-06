using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

namespace ResourceFormats
{
    public class TableResource : IResourceType
    {
        public List<TableData> Tables = new List<TableData>();

        public string Name; // M-3

        public void Serialize(ushort version, Stream input, Endian endian)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            var NameHash = input.ReadValueU64(endian);
            var DataSize = input.ReadValueU32(endian);
            var Hash = input.ReadValueU64(endian); // DataHash?
            var Unk1 = input.ReadValueU32(endian); // 4

            input.ReadBytes(DataSize);

            var TableMP = input.ReadStringU32(endian);
            var HashName = input.ReadValueU64(endian); // if TableMP == null HashName = 0
            Name = input.ReadStringU32(endian);

            string name = Path.GetFileNameWithoutExtension(Name);
            if (FileFormats.Hashing.FNV64.Hash(name) != NameHash)
            {
                System.Diagnostics.Debugger.Break();
            }

            /*uint count = input.ReadValueU32(endian);
            Tables.Clear();
            for (uint i = 0; i < count; i++)
            {
                var table = new TableData();
                table.Deserialize(version, input, endian);
                Tables.Add(table);
            }*/
        }
    }
}
