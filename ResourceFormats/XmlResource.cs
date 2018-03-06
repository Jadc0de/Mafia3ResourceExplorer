using System.IO;
using FileFormats;
using Gibbed.IO;

namespace ResourceFormats
{
    public class XmlResource : IResourceType
    {
        public string Tag;
        public byte Unk1;
        public string Name;
        public byte Unk3;

        public string Content;

        public void Serialize(ushort version, Stream output, Endian endian)
        {
            output.WriteStringU32(Tag, endian);
            output.WriteValueU8(Unk1);
            output.WriteStringU32(Name, endian);
            output.WriteValueU8(Unk3);

            if (Unk3 == 0)
            {
                XmlResource0.Serialize(output, Content, endian);
            }
            else
            {
                XmlResource1.Serialize(output, Content, endian);
            }
        }

        public void Deserialize(ushort version, Stream input, Endian endian)
        {
            Tag = input.ReadStringU32(endian);
            Unk1 = input.ReadValueU8();         // 0 1
            Name = input.ReadStringU32(endian);
            Unk3 = input.ReadValueU8();         // 0 1

            if (Unk1 != 0 && Unk1 != 1 && Unk3 != 0 && Unk3 != 1)
            {
                throw new System.FormatException();
            }

            if (Unk3 == 0)
            {
                Content = XmlResource0.Deserialize(input, endian);
            }
            else
            {
                Content = XmlResource1.Deserialize(input, endian);
            }
        }

    }
}
