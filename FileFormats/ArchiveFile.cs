using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gibbed.IO;

namespace FileFormats
{
    public class ArchiveFile : IDisposable
    {
        public const uint Signature = 0x53445300; // 'SDS\0'

        private Endian _Endian;
        private Archive.Platform _Platform;
        private uint _SlotRamRequired;
        private uint _SlotVramRequired;
        private uint _OtherRamRequired;
        private uint _OtherVramRequired;
        private byte[] _Unknown20;
        private readonly List<Archive.ResourceType> _ResourceTypes;
        private string _ResourceInfoXml;
        private readonly List<Archive.ResourceEntry> _ResourceEntries;

        public ArchiveFile()
        {
            _ResourceTypes = new List<Archive.ResourceType>();
            _ResourceEntries = new List<Archive.ResourceEntry>();
        }

        public Endian Endian
        {
            get { return _Endian; }
            set { _Endian = value; }
        }

        public Archive.Platform Platform
        {
            get { return _Platform; }
            set { _Platform = value; }
        }

        public uint SlotRamRequired
        {
            get { return _SlotRamRequired; }
            set { _SlotRamRequired = value; }
        }

        public uint SlotVramRequired
        {
            get { return _SlotVramRequired; }
            set { _SlotVramRequired = value; }
        }

        public uint OtherRamRequired
        {
            get { return _OtherRamRequired; }
            set { _OtherRamRequired = value; }
        }

        public uint OtherVramRequired
        {
            get { return _OtherVramRequired; }
            set { _OtherVramRequired = value; }
        }

        public byte[] Unknown20
        {
            get { return _Unknown20; }
            set { _Unknown20 = value; }
        }

        public List<Archive.ResourceType> ResourceTypes
        {
            get { return _ResourceTypes; }
        }

        public string ResourceInfoXml
        {
            get { return _ResourceInfoXml; }
            set { _ResourceInfoXml = value; }
        }

        public List<Archive.ResourceEntry> ResourceEntries
        {
            get { return _ResourceEntries; }
        }

        public void Serialize(Stream output, Options options)
        {
            var compress = (options & Options.Compress) != 0;

            var basePosition = output.Position;
            var endian = _Endian;

            using (var data = new MemoryStream(12))
            {
                data.WriteValueU32(Signature, Endian.Big);
                // Mafia II  = 19
                // Mafia III = 20
                data.WriteValueU32(20, endian);
                data.WriteValueU32((uint)_Platform, Endian.Big);
                data.Flush();
                output.WriteFromMemoryStreamSafe(data, endian);
            }

            var headerPosition = output.Position;

            Archive.FileHeader fileHeader;
            output.Seek(56, SeekOrigin.Current);

            fileHeader.ResourceTypeTableOffset = (uint)(output.Position - basePosition);
            output.WriteValueS32(_ResourceTypes.Count, endian);
            foreach (var resourceType in _ResourceTypes)
            {
                resourceType.Write(output, endian);
            }

            /*var blockAlignment = (options & ArchiveSerializeOptions.OneBlock) != 0
                ? (uint)this._ResourceEntries.Sum(re => 30 + (re.Data == null ? 0 : re.Data.Length))
                : 0x4000;
            */

            var blockAlignment = (options & Options.OneBlock) != 0 ? (uint)_ResourceEntries.Sum(re => 38 + (re.Data == null ? 0 : re.Data.Length)) : 0x00010000;

            fileHeader.BlockTableOffset = (uint)(output.Position - basePosition);
            fileHeader.ResourceCount = 0;
            var blockStream = BlockWriterStream.ToStream(output, blockAlignment, endian, compress);
            foreach (var resourceEntry in _ResourceEntries)
            {
                Archive.ResourceHeader resourceHeader;
                resourceHeader.TypeId = resourceEntry.TypeId;
                // resourceHeader.Size = 30 + (uint)(resourceEntry.Data == null ? 0 : resourceEntry.Data.Length);
                resourceHeader.Size = 38 + (uint)(resourceEntry.Data == null ? 0 : resourceEntry.Data.Length); // M3
                resourceHeader.Version = resourceEntry.Version;
                resourceHeader.SlotRamRequired = resourceEntry.SlotRamRequired;
                resourceHeader.SlotVramRequired = resourceEntry.SlotVramRequired;
                resourceHeader.OtherRamRequired = resourceEntry.OtherRamRequired;
                resourceHeader.OtherVramRequired = resourceEntry.OtherVramRequired;

                resourceHeader.Unknown1 = resourceEntry.Unknown1; // M3
                resourceHeader.Unknown2 = resourceEntry.Unknown2; // M3

                using (var data = new MemoryStream())
                {
                    resourceHeader.Write(data, endian);
                    data.Flush();
                    blockStream.WriteFromMemoryStreamSafe(data, endian);
                }

                blockStream.WriteBytes(resourceEntry.Data);
                fileHeader.ResourceCount++;
            }
            blockStream.Flush();
            blockStream.Finish();

            fileHeader.XmlOffset = (uint)(output.Position - basePosition);
            if (string.IsNullOrEmpty(_ResourceInfoXml) == false)
            {
                output.WriteString(_ResourceInfoXml, Encoding.ASCII);
            }
            else
            {
                fileHeader.XmlOffset = 0;
            }

            fileHeader.SlotRamRequired = SlotRamRequired;
            fileHeader.SlotVramRequired = SlotVramRequired;
            fileHeader.OtherRamRequired = OtherRamRequired;
            fileHeader.OtherVramRequired = OtherVramRequired;
            fileHeader.Flags = 1;
            fileHeader.Unknown20 = _Unknown20 ?? new byte[16];

            output.Position = headerPosition;
            using (var data = new MemoryStream())
            {
                fileHeader.Write(data, endian);
                data.Flush();
                output.WriteFromMemoryStreamSafe(data, endian);
            }
        }

        public void SerializeM2(Stream output, Options options)
        {
            var compress = (options & Options.Compress) != 0;

            var basePosition = output.Position;
            var endian = _Endian;

            using (var data = new MemoryStream(12))
            {
                data.WriteValueU32(Signature, Endian.Big);
                // Mafia II  = 19
                // Mafia III = 20
                data.WriteValueU32(20, endian);
                data.WriteValueU32((uint)_Platform, Endian.Big);
                data.Flush();
                output.WriteFromMemoryStreamSafe(data, endian);
            }

            var headerPosition = output.Position;

            Archive.FileHeader fileHeader;
            output.Seek(56, SeekOrigin.Current);

            fileHeader.ResourceTypeTableOffset = (uint)(output.Position - basePosition);
            output.WriteValueS32(_ResourceTypes.Count, endian);
            foreach (var resourceType in _ResourceTypes)
            {
                resourceType.Write(output, endian);
            }

            /*var blockAlignment = (options & ArchiveSerializeOptions.OneBlock) != 0
                ? (uint)this._ResourceEntries.Sum(re => 30 + (re.Data == null ? 0 : re.Data.Length))
                : 0x4000;
            */

            var blockAlignment = (options & Options.OneBlock) != 0 ? (uint)_ResourceEntries.Sum(re => 38 + (re.Data == null ? 0 : re.Data.Length)) : 0x00010000;

            fileHeader.BlockTableOffset = (uint)(output.Position - basePosition);
            fileHeader.ResourceCount = 0;

            var blockStream = new MemoryStream();
            foreach (var resourceEntry in _ResourceEntries)
            {
                Archive.ResourceHeader resourceHeader;
                resourceHeader.TypeId = resourceEntry.TypeId;
                // resourceHeader.Size = 30 + (uint)(resourceEntry.Data == null ? 0 : resourceEntry.Data.Length);
                resourceHeader.Size = 38 + (uint)(resourceEntry.Data == null ? 0 : resourceEntry.Data.Length); // M3
                resourceHeader.Version = resourceEntry.Version;
                resourceHeader.SlotRamRequired = resourceEntry.SlotRamRequired;
                resourceHeader.SlotVramRequired = resourceEntry.SlotVramRequired;
                resourceHeader.OtherRamRequired = resourceEntry.OtherRamRequired;
                resourceHeader.OtherVramRequired = resourceEntry.OtherVramRequired;

                resourceHeader.Unknown1 = resourceEntry.Unknown1; // M3
                resourceHeader.Unknown2 = resourceEntry.Unknown2; // M3

                using (var data = new MemoryStream())
                {
                    resourceHeader.Write(data, endian);
                    data.Flush();
                    blockStream.WriteFromMemoryStreamSafe(data, endian);
                }

                blockStream.WriteBytes(resourceEntry.Data);
                fileHeader.ResourceCount++;
            }
            blockStream.Position = 0L;

            //
            if (compress)
            {
                var level = ZLibNet.CompressionLevel.BestCompression;
                List<Block> blocks = UEzlCompressBytes(blockStream.ToArray(), level, blockAlignment);
                output.WriteValueU32(0x6C7A4555); // zlEU
                output.WriteValueU32(blockAlignment);
                output.WriteValueU8(4);
                for (int i = 0; i < blocks.Count; i++)
                {
                    output.WriteValueS32(blocks[i].Size);
                    output.WriteValueU8(1);
                    output.WriteValueS32(blocks[i].UncompressedSize);
                    output.WriteValueU32(32);
                    output.WriteValueU32(blockAlignment);
                    output.WriteValueU32(135200769);
                    output.WriteValueS32(blocks[i].CompressedSize);
                    output.WriteValueU32(0u);
                    output.WriteValueU32(0u);
                    output.WriteValueU32(0u);
                    output.Write(blocks[i].ZBuffer, 0, blocks[i].ZBuffer.Length);
                }
            }
            else
            {
                output.WriteValueU32(0x6C7A4555); // zlEU
                output.WriteValueU32((uint)blockStream.Length);
                output.WriteValueU8(4);
                output.WriteValueU32((uint)blockStream.Length);
                output.WriteValueU8(0);
                output.WriteFromStream(blockStream, (long)((ulong)((uint)blockStream.Length)));
            }
            output.WriteValueU32(0u);
            output.WriteValueU8(0);
            //

            fileHeader.XmlOffset = (uint)(output.Position - basePosition);
            if (string.IsNullOrEmpty(_ResourceInfoXml) == false)
            {
                output.WriteString(_ResourceInfoXml, Encoding.ASCII);
            }
            else
            {
                fileHeader.XmlOffset = 0;
            }

            fileHeader.SlotRamRequired = SlotRamRequired;
            fileHeader.SlotVramRequired = SlotVramRequired;
            fileHeader.OtherRamRequired = OtherRamRequired;
            fileHeader.OtherVramRequired = OtherVramRequired;
            fileHeader.Flags = 1;
            fileHeader.Unknown20 = _Unknown20 ?? new byte[16];

            output.Position = headerPosition;
            using (var data = new MemoryStream())
            {
                fileHeader.Write(data, endian);
                data.Flush();
                output.WriteFromMemoryStreamSafe(data, endian);
            }
        }

        private class Block
        {
            public int Size;

            public bool Compressed;

            public int UncompressedSize;

            public uint Unknown04;

            public uint Unknown08;

            public uint Unknown0C;

            public int CompressedSize;

            public uint Unknown14;

            public uint Unknown18;

            public uint Unknown1C;

            public byte[] ZBuffer;
        }

        private byte[] UEzlCompress(byte[] input, ZLibNet.CompressionLevel level)
        {
            byte[] buffer = null;
            using (var data = new MemoryStream())
            {
                var zlib = new ZLibNet.ZLibStream(data, ZLibNet.CompressionMode.Compress, ZLibNet.CompressionLevel.BestCompression);
                zlib.Write(input, 0, input.Length);
                zlib.Flush();
                buffer = data.GetBuffer();
            }
            return buffer;
        }

        private List<Block> UEzlCompressBytes(byte[] input, ZLibNet.CompressionLevel level, uint alignment)
        {
            List<List<byte>> split = SplitByteArray(input, alignment);
            List<Block> _return = new List<Block>();
            for (int i = 0; i < split.Count; i++)
            {
                byte[] array = split[i].ToArray();
                byte[] array2 = UEzlCompress(array, level);
                _return.Add(new Block
                {
                    Size = array2.Length + 32,
                    Compressed = true,
                    UncompressedSize = array.Length,
                    Unknown04 = 32u,
                    Unknown08 = 81920u,
                    Unknown0C = 135200769u,
                    CompressedSize = array2.Length,
                    Unknown14 = 0u,
                    Unknown18 = 0u,
                    Unknown1C = 0u,
                    ZBuffer = array2
                });

                array = null;
                array2 = null;

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return _return;
        }

        private List<List<byte>> SplitByteArray(byte[] buffer, uint alignment)
        {
            List<byte> list = new List<byte>();
            List<List<byte>> list2 = new List<List<byte>>();
            uint num = 1u;
            while ((ulong)num < (ulong)((long)(buffer.Length + 1)))
            {
                list.Add(buffer[(int)((UIntPtr)(num - 1u))]);
                if (num % alignment == 0u)
                {
                    list2.Add(list);
                    list = new List<byte>();
                }
                num += 1u;
            }
            list2.Add(list);
            return list2;
        }


        public void Deserialize(Stream input)
        {
            var basePosition = input.Position;

            var magic = input.ReadValueU32(Endian.Big);
            if (magic != Signature)
            {
                throw new FormatException("unsupported archive version");
            }

            // Mafia II  = 13
            // Mafia III = 14
            var vers = input.ReadValueU32(Endian.Big);

            var platform = (Archive.Platform)input.ReadValueU32(Endian.Big);
            if (platform != Archive.Platform.PC &&
                platform != Archive.Platform.Xbox360 &&
                platform != Archive.Platform.PS3)
            {
                throw new FormatException("unsupported archive platform");
            }
            var endian = platform == Archive.Platform.PC ? Endian.Little : Endian.Big;

            input.Position = basePosition;

            uint version;
            using (var data = input.ReadToMemoryStreamSafe(12, endian))
            {
                data.Position += 4; // skip magic
                version = data.ReadValueU32(endian);
                data.Position += 4; // skip platform
            }

            // Mafia II  = 19
            // Mafia III = 20
            if (version != 19 && version != 20)
            {
                throw new FormatException("unsupported archive version");
            }

            Archive.FileHeader fileHeader;
            using (var data = input.ReadToMemoryStreamSafe(52, endian))
            {
                fileHeader = Archive.FileHeader.Read(data, endian);
            }

            input.Position = basePosition + fileHeader.ResourceTypeTableOffset;
            var resourceTypeCount = input.ReadValueU32(endian);
            var resourceTypes = new Archive.ResourceType[resourceTypeCount];
            for (uint i = 0; i < resourceTypeCount; i++)
            {
                resourceTypes[i] = Archive.ResourceType.Read(input, endian);
            }

            input.Position = basePosition + fileHeader.BlockTableOffset;
            var blockStream = BlockReaderStream.FromStream(input, endian);

            var resources = new Archive.ResourceEntry[fileHeader.ResourceCount];
            for (uint i = 0; i < fileHeader.ResourceCount; i++)
            {
                Archive.ResourceHeader resourceHeader;
                //using (var data = blockStream.ReadToMemoryStreamSafe(26, endian))
                using (var data = blockStream.ReadToMemoryStreamSafe(34, endian)) // M3
                {
                    resourceHeader = Archive.ResourceHeader.Read(data, endian);
                }

                //if (resourceHeader.Size < 30)
                if (resourceHeader.Size < 38) // + XmlOffset
                {
                    throw new FormatException();
                }

                resources[i] = new Archive.ResourceEntry()
                {
                    TypeId = resourceHeader.TypeId,
                    Version = resourceHeader.Version,
                    //Data = blockStream.ReadBytes(resourceHeader.Size - 30),
                    Data = blockStream.ReadBytes(resourceHeader.Size - 38),
                    SlotRamRequired = resourceHeader.SlotRamRequired,
                    SlotVramRequired = resourceHeader.SlotVramRequired,
                    OtherRamRequired = resourceHeader.OtherRamRequired,
                    OtherVramRequired = resourceHeader.OtherVramRequired,
                    Unknown1 = resourceHeader.Unknown1,
                    Unknown2 = resourceHeader.Unknown2,
                };
            }

            string xml = null;
            if (fileHeader.XmlOffset > 0)
            {
                input.Position = basePosition + fileHeader.XmlOffset;
                xml = input.ReadString((int)(input.Length - input.Position), Encoding.ASCII);
            }

            _ResourceTypes.Clear();
            _ResourceEntries.Clear();

            _Endian = endian;
            _Platform = platform;
            _SlotRamRequired = fileHeader.SlotRamRequired;
            _SlotVramRequired = fileHeader.SlotVramRequired;
            _OtherRamRequired = fileHeader.OtherRamRequired;
            _OtherVramRequired = fileHeader.OtherVramRequired;
            _Unknown20 = (byte[])fileHeader.Unknown20.Clone();
            _ResourceTypes.AddRange(resourceTypes);
            _ResourceInfoXml = xml;
            _ResourceEntries.AddRange(resources);
        }


        private bool disposed = false;

        //реализация интерфейса IDisposable
        //не делайте эту функцию виртуальной
        //и не перекрывайте в потомках
        public void Dispose()
        {
            Dispose(true);       //освобождение вызвали вручную
            GC.SuppressFinalize(this); //не хотим, чтобы GC вызвал деструктор
        }

        //деструктор
        ~ArchiveFile()
        {
            Dispose(false); //освобождения ресурсов потребовал GC вызвав деструтор
        }

        //освобождаем ресурсы
        private void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    //освобождаем managed ресурсы
                    //...

                    //иными словами - вызываем метод Dispose
                    //для всех managed член-переменных класса
                    //это нужно делать только для ручного вызова Dispose,
                    //потому что в другом случае случае Dispose для них вызовет GC

                    ResourceTypes.Clear();
                    foreach (var entry in ResourceEntries)
                    {
                        entry.Data = null;
                    }
                    ResourceEntries.Clear();
                }

                //освобождаем unmanaged ресурсы
                //...

                disposed = true;
            }
        }
    }

}
