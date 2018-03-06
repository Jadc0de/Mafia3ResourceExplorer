using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

namespace ResourceFormats
{
    public class ScriptResource : IResourceType
    {
        public string Path;
        public List<ScriptData> Scripts = new List<ScriptData>();

        public void Serialize(ushort version, Stream output, Endian endian)
        {
            output.WriteStringU16(Path, endian);
            output.WriteValueS32(Scripts.Count, endian);
            foreach (var script in Scripts)
            {
                script.Serialize(version, output, endian);
            }
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            Path = input.ReadStringU16(endian);
            var count = input.ReadValueU32(endian);
            Scripts.Clear();
            for (uint i = 0; i < count; i++)
            {
                var script = new ScriptData();
                script.Deserialize(version, input, endian);
                Scripts.Add(script);
            }
        }
    }
}
