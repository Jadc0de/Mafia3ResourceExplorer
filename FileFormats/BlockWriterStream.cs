using System;
using System.IO;
using Gibbed.IO;
//using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ZLibNet;

namespace FileFormats
{
    public class BlockWriterStream : Stream
    {
        public const uint Signature = 0x6C7A4555; // 'zlEU'

        private readonly Endian _Endian;
        private readonly uint _Alignment;
        private readonly Stream _BaseStream;
        private readonly byte[] _BlockBytes;
        private int _BlockOffset;
        private readonly bool _IsCompressing;

        private BlockWriterStream(Stream baseStream, uint alignment, Endian endian, bool compress)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }

            _BaseStream = baseStream;
            _Alignment = alignment;
            _Endian = endian;
            _BlockBytes = new byte[alignment];
            _BlockOffset = 0;
            _IsCompressing = compress;
        }

        #region Stream
        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            FlushBlock();
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            long remaining = count;
            while (remaining > 0)
            {
                var write = (int)Math.Min(remaining, _Alignment - _BlockOffset);
                Array.Copy(buffer, offset, _BlockBytes, _BlockOffset, write);
                _BlockOffset += write;
                remaining -= write;
                offset += write;

                if (_BlockOffset >= _Alignment)
                {
                    FlushBlock();
                }
            }
        }

        private void FlushBlock()
        {
            if (_BlockOffset == 0)
            {
                return;
            }

            if (_IsCompressing == false || FlushCompressedBlock() == false)
            {
                /*var blockLength = Array.FindLastIndex(this._BlockBytes, this._BlockOffset - 1, b => b != 0);
                blockLength = 1 + (blockLength < 0 ? 0 : blockLength);*/
                var blockLength = _BlockOffset;
                _BaseStream.WriteValueS32(blockLength, _Endian);
                _BaseStream.WriteValueU8(0);
                _BaseStream.Write(_BlockBytes, 0, blockLength);
                _BlockOffset = 0;
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

            public void Write(Stream output, Endian endian)
            {
                output.WriteValueU32(UncompressedSize, endian);
                output.WriteValueU32(Unknown04, endian);
                output.WriteValueU32(Unknown08, endian);
                output.WriteValueU32(Unknown0C, endian);
                output.WriteValueU32(CompressedSize, endian);
                output.WriteValueU32(Unknown14, endian);
                output.WriteValueU32(Unknown18, endian);
                output.WriteValueU32(Unknown1C, endian);
            }
        }

        private bool FlushCompressedBlock()
        {
            using (var data = new MemoryStream())
            {
                /*var blockLength = Array.FindLastIndex(this._BlockBytes, this._BlockOffset - 1, b => b != 0);
                blockLength = 1 + (blockLength < 0 ? 0 : blockLength);*/
                var blockLength = _BlockOffset;

                // var zlib = new DeflaterOutputStream(data);

                /*ICSharpCode.SharpZipLib.Zip.Compression.Deflater deflater = new ICSharpCode.SharpZipLib.Zip.Compression.Deflater(9);
                var zlib = new DeflaterOutputStream(data, deflater);
                zlib.Write(this._BlockBytes, 0, blockLength);
                zlib.Finish();
                data.Flush();*/

                var zlib = new ZLibStream(data, CompressionMode.Compress, CompressionLevel.BestCompression);
                zlib.Write(_BlockBytes, 0, blockLength);
                zlib.Flush();
                //data.Flush();

                var compressedLength = (int)data.Length;
                if (data.Length < blockLength)
                {
                    _BaseStream.WriteValueS32(32 + compressedLength, _Endian);
                    _BaseStream.WriteValueU8(1);
                    CompressedBlockHeader compressedBlockHeader;
                    compressedBlockHeader.UncompressedSize = (uint)blockLength;
                    compressedBlockHeader.Unknown04 = 32;
                    // II  = 81920
                    // III = 65536
                    // compressedBlockHeader.Unknown08 = 65536; // this._Alignment
                    compressedBlockHeader.Unknown08 = _Alignment;
                    compressedBlockHeader.Unknown0C = 135200769;
                    compressedBlockHeader.CompressedSize = (uint)compressedLength;
                    compressedBlockHeader.Unknown14 = 0;
                    compressedBlockHeader.Unknown18 = 0;
                    compressedBlockHeader.Unknown1C = 0;
                    compressedBlockHeader.Write(_BaseStream, _Endian);
                    _BaseStream.Write(data.GetBuffer(), 0, compressedLength);
                    _BlockOffset = 0;

                    zlib.Close();
                    zlib.Dispose();

                    return true;
                }
            }
            return false;
        }

        public void Finish()
        {
            _BaseStream.WriteValueS32(0, _Endian);
            _BaseStream.WriteValueU8(0);
        }
        #endregion

        public static BlockWriterStream ToStream(Stream baseStream, uint alignment, Endian endian, bool compress)
        {
            var instance = new BlockWriterStream(baseStream, alignment, endian, compress);
            baseStream.WriteValueU32(Signature, endian);
            baseStream.WriteValueU32(alignment, endian);
            baseStream.WriteValueU8(4);
            return instance;
        }
    }
}
