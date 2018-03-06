using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Gibbed.IO;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ZLibNet;

namespace FileFormats
{
    public class BlockReaderStream : Stream
    {
        private readonly Stream _BaseStream;
        private readonly List<Block> _Blocks;
        private Block _CurrentBlock;
        private long _CurrentPosition;

        private BlockReaderStream(Stream baseStream)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }

            _BaseStream = baseStream;
            _Blocks = new List<Block>();
            _CurrentPosition = 0;
        }

        public void FreeLoadedBlocks()
        {
            foreach (var block in _Blocks)
            {
                block.FreeLoadedData();
            }
        }

        private void AddUncompressedBlock(long virtualOffset, uint virtualSize, long dataOffset)
        {
            _Blocks.Add(new UncompressedBlock(virtualOffset, virtualSize, dataOffset));
        }

        private void AddCompressedBlock(long virtualOffset, uint virtualSize, long dataOffset, uint dataCompressedSize)
        {
            _Blocks.Add(new CompressedBlock(virtualOffset, virtualSize, dataOffset, dataCompressedSize));
        }

        private bool LoadBlock(long offset)
        {
            if (_CurrentBlock == null || _CurrentBlock.IsValidOffset(offset) == false)
            {
                Block block = _Blocks.SingleOrDefault(candidate => candidate.IsValidOffset(offset));
                if (block == null)
                {
                    _CurrentBlock = null;
                    return false;
                }
                _CurrentBlock = block;
            }

            return _CurrentBlock.Load(_BaseStream);
        }

        public void SaveUncompressed(Stream output)
        {
            byte[] data = new byte[1024];

            long totalSize = _Blocks.Max(candidate => candidate.Offset + candidate.Size);

            output.SetLength(totalSize);

            foreach (Block block in _Blocks)
            {
                output.Seek(block.Offset, SeekOrigin.Begin);
                Seek(block.Offset, SeekOrigin.Begin);

                long total = block.Size;
                while (total > 0)
                {
                    int read = Read(data, 0, (int)Math.Min(total, data.Length));
                    output.Write(data, 0, read);
                    total -= read;
                }
            }

            output.Flush();
        }

        #region Stream
        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            _BaseStream.Flush();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { return _CurrentPosition; }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalRead = 0;

            while (totalRead < count)
            {
                if (LoadBlock(_CurrentPosition) == false)
                {
                    throw new InvalidOperationException();
                }

                int read = _CurrentBlock.Read(
                    _BaseStream,
                    _CurrentPosition,
                    buffer,
                    offset + totalRead,
                    count - totalRead);

                totalRead += read;
                _CurrentPosition += read;
            }

            return totalRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.End)
            {
                throw new NotSupportedException();
            }

            if (origin == SeekOrigin.Current)
            {
                if (offset == 0)
                {
                    return _CurrentPosition;
                }

                offset = _CurrentPosition + offset;
            }

            /*
            :effort: in fixing seeks that hit the end of data instead of over it
            if (this.LoadBlock(offset) == false)
            {
                throw new InvalidOperationException();
            }
            */

            _CurrentPosition = offset;
            return _CurrentPosition;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
        #endregion

        private abstract class Block
        {
            public long Offset { get; protected set; }
            public uint Size { get; protected set; }

            public Block(long offset, uint size)
            {
                Offset = offset;
                Size = size;
            }

            public bool IsValidOffset(long offset)
            {
                return offset >= Offset &&
                       offset < Offset + Size;
            }

            public abstract bool Load(Stream input);
            public abstract void FreeLoadedData();
            public abstract int Read(Stream input, long baseOffset, byte[] buffer, int offset, int count);
        }

        private class UncompressedBlock : Block
        {
            private readonly long _DataOffset;
            private bool _IsLoaded;
            private byte[] _Data;

            public UncompressedBlock(long virtualOffset, uint virtualSize, long dataOffset)
                : base(virtualOffset, virtualSize)
            {
                _DataOffset = dataOffset;
                _IsLoaded = false;
                _Data = null;
            }

            public override void FreeLoadedData()
            {
                _IsLoaded = false;
                _Data = null;
            }

            public override bool Load(Stream input)
            {
                if (_IsLoaded == true)
                {
                    return true;
                }

                input.Seek(_DataOffset, SeekOrigin.Begin);
                _Data = new byte[Size];
                if (input.Read(_Data, 0, _Data.Length) != _Data.Length)
                {
                    throw new InvalidOperationException();
                }

                _IsLoaded = true;
                return true;
            }

            public override int Read(Stream input, long baseOffset, byte[] buffer, int offset, int count)
            {
                if (baseOffset >= Offset + Size)
                {
                    return 0;
                }

                Load(input);

                int relativeOffset = (int)(baseOffset - Offset);
                int read = (int)Math.Min(Size - relativeOffset, count);
                Array.ConstrainedCopy(_Data, relativeOffset, buffer, offset, read);
                return read;
            }
        }

        private class CompressedBlock : Block
        {
            private readonly long _DataOffset;
            private uint _DataCompressedSize;
            private bool _IsLoaded;
            private byte[] _Data;

            public CompressedBlock(long virtualOffset, uint virtualSize, long dataOffset, uint dataCompressedSize)
                : base(virtualOffset, virtualSize)
            {
                _DataOffset = dataOffset;
                _DataCompressedSize = dataCompressedSize;

                _IsLoaded = false;
                _Data = null;
            }

            public override void FreeLoadedData()
            {
                _IsLoaded = false;
                _Data = null;
            }

            public override bool Load(Stream input)
            {
                if (_IsLoaded == true)
                {
                    return true;
                }

                /*input.Seek(this._DataOffset, SeekOrigin.Begin);
                this._Data = new byte[this.Size];

                var inflater = new InflaterInputStream(input);
                if (inflater.Read(this._Data, 0, this._Data.Length) != this._Data.Length)
                {
                    throw new InvalidOperationException();
                }*/

                input.Seek(_DataOffset, SeekOrigin.Begin);
                _Data = new byte[Size];

                using (ZLibStream _ZLibStream = new ZLibStream(input, CompressionMode.Decompress, true))
                {
                    if (_ZLibStream.Read(_Data, 0, _Data.Length) != _Data.Length)
                    {
                        throw new InvalidOperationException();
                    }
                }

                _IsLoaded = true;
                return true;
            }

            public override int Read(Stream input, long baseOffset, byte[] buffer, int offset, int count)
            {
                if (baseOffset >= Offset + Size)
                {
                    return 0;
                }

                Load(input);

                int relativeOffset = (int)(baseOffset - Offset);
                int read = (int)Math.Min(Size - relativeOffset, count);
                Array.ConstrainedCopy(_Data, relativeOffset, buffer, offset, read);
                return read;
            }
        }

        private struct CompressedBlockHeader
        {
            public uint UncompressedSize;
            public uint Unknown04;
            public uint Unknown08;
            public uint Unknown0C;
            public uint CompressedSize;
            public uint Unknown14;
            public uint Unknown18;
            public uint Unknown1C;

            public static CompressedBlockHeader Read(Stream input, Endian endian)
            {
                CompressedBlockHeader instance;
                instance.UncompressedSize = input.ReadValueU32(endian);
                instance.Unknown04 = input.ReadValueU32(endian);
                instance.Unknown08 = input.ReadValueU32(endian);
                instance.Unknown0C = input.ReadValueU32(endian);
                instance.CompressedSize = input.ReadValueU32(endian);
                instance.Unknown14 = input.ReadValueU32(endian);
                instance.Unknown18 = input.ReadValueU32(endian);
                instance.Unknown1C = input.ReadValueU32(endian);
                return instance;
            }
        }

        public const uint Signature = 0x6C7A4555; // 'zlEU'

        public static BlockReaderStream FromStream(Stream baseStream, Endian endian)
        {
            var instance = new BlockReaderStream(baseStream);

            var magic = baseStream.ReadValueU32(endian);
            var alignment = baseStream.ReadValueU32(endian); // III = 0x00010000
            var flags = baseStream.ReadValueU8();

            if (magic != Signature || /*alignment != 0x4000 ||*/ flags != 4)
            {
                throw new InvalidOperationException();
            }

            long virtualOffset = 0;
            while (true)
            {
                uint size = baseStream.ReadValueU32(endian);
                bool isCompressed = baseStream.ReadValueU8() != 0;

                if (size == 0)
                {
                    break;
                }

                if (isCompressed == true)
                {
                    var compressedBlockHeader = CompressedBlockHeader.Read(baseStream, endian);
                    /*if (compressedBlockHeader.Unknown04 != 32 ||
                        compressedBlockHeader.Unknown08 != 81920 ||
                        compressedBlockHeader.Unknown0C != 135200769 ||
                        compressedBlockHeader.Unknown14 != 0 ||
                        compressedBlockHeader.Unknown18 != 0 ||
                        compressedBlockHeader.Unknown1C != 0)
                    {
                        throw new InvalidOperationException();
                    }*/

                    // 32        == 0x00000020
                    // 65536     == 0x00010000
                    // 135200769 == 0x080f0001

                    if (compressedBlockHeader.Unknown04 != 32 ||
                        compressedBlockHeader.Unknown08 != 65536 && compressedBlockHeader.Unknown08 != compressedBlockHeader.UncompressedSize ||
                        compressedBlockHeader.Unknown0C != 135200769 ||
                        compressedBlockHeader.Unknown14 != 0 ||
                        compressedBlockHeader.Unknown18 != 0 ||
                        compressedBlockHeader.Unknown1C != 0 )
                    {
                        throw new InvalidOperationException();
                    }


                    if (size - 32 != compressedBlockHeader.CompressedSize)
                    {
                        throw new InvalidOperationException();
                    }

                    instance.AddCompressedBlock(virtualOffset,
                                                compressedBlockHeader.UncompressedSize,
                                                baseStream.Position,
                                                compressedBlockHeader.CompressedSize);
                    baseStream.Seek(compressedBlockHeader.CompressedSize, SeekOrigin.Current);
                }
                else
                {
                    instance.AddUncompressedBlock(virtualOffset, size, baseStream.Position);
                    baseStream.Seek(size, SeekOrigin.Current);
                }

                virtualOffset += alignment;
            }

            return instance;
        }
    }
}
