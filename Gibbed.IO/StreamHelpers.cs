/* Copyright (c) 2017 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.IO;
using System.Text;

namespace Gibbed.IO
{
    public static partial class StreamHelpers
    {
        static StreamHelpers()
        {
            IO.StreamHelpers.DefaultEncoding = Encoding.GetEncoding(1252);
        }

        internal static bool ShouldSwap(Endian endian)
        {
            switch (endian)
            {
                case Endian.Little: return BitConverter.IsLittleEndian == false;
                case Endian.Big: return BitConverter.IsLittleEndian == true;
                default: throw new ArgumentException("unsupported endianness", "endian");
            }
        }

        public static MemoryStream ReadToMemoryStream(this Stream stream, long size, int buffer)
        {
            var memory = new MemoryStream();

            long left = size;
            var data = new byte[buffer];
            while (left > 0)
            {
                var block = (int)(Math.Min(left, data.Length));
                if (stream.Read(data, 0, block) != block)
                {
                    throw new EndOfStreamException();
                }
                memory.Write(data, 0, block);
                left -= block;
            }

            memory.Seek(0, SeekOrigin.Begin);
            return memory;
        }

        public static MemoryStream ReadToMemoryStream(this Stream stream, long size)
        {
            return stream.ReadToMemoryStream(size, 0x40000);
        }

        public static void WriteFromStream(this Stream stream, Stream input, long size, int buffer)
        {
            long left = size;
            var data = new byte[buffer];
            while (left > 0)
            {
                var block = (int)(Math.Min(left, data.Length));
                var read = input.Read(data, 0, block);
                if (read != block)
                {
                    throw new EndOfStreamException();
                }
                stream.Write(data, 0, block);
                left -= block;
            }
        }

        public static void WriteFromStream(this Stream stream, Stream input, long size)
        {
            stream.WriteFromStream(input, size, 0x40000);
        }

        public static byte[] ReadBytes(this Stream stream, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            var data = new byte[length];
            var read = stream.Read(data, 0, length);
            if (read != length)
            {
                throw new EndOfStreamException();
            }

            return data;
        }

        public static byte[] ReadBytes(this Stream stream, uint length)
        {
            return stream.ReadBytes((int)length);
        }

        public static void WriteBytes(this Stream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        public static MemoryStream ReadToMemoryStreamSafe(this Stream stream, long size, Endian endian)
        {
            var output = new MemoryStream();

            uint computedHash = FileFormats.Hashing.FNV32.Initial;
            long remaining = size;
            byte[] buffer = new byte[4096];
            while (remaining > 0)
            {
                int block = (int)(Math.Min(remaining, 4096));
                var read = stream.Read(buffer, 0, block);
                if (read != block)
                {
                    throw new EndOfStreamException();
                }

                computedHash = FileFormats.Hashing.FNV32.Hash(buffer, 0, block);
                output.Write(buffer, 0, block);
                remaining -= block;
            }

            // 0x5de53fde 0x2a930c2e
            var hash = stream.ReadValueU32(endian);
            if (hash != computedHash)
            {
                throw new InvalidDataException(string.Format("hash failure ({0:X} vs {1:X})", computedHash, hash));
            }

            output.Position = 0;
            return output;
        }

        public static void WriteFromMemoryStreamSafe(this Stream stream, MemoryStream input, Endian endian)
        {
            var position = input.Position;
            input.Position = 0;
            var buffer = input.GetBuffer();
            var length = (int)input.Length;
            var computedHash = FileFormats.Hashing.FNV32.Hash(buffer, 0, length);
            stream.Write(buffer, 0, length);
            stream.WriteValueU32(computedHash, endian);
            input.Position = position;
        }

        public static string ReadStringU8(this Stream stream, byte seek = 0)
        {
            return stream.ReadStringU8(true, seek);
        }

        public static string ReadStringU8(this Stream stream, bool littleEndian, byte seek = 0)
        {
            var length = stream.ReadValueU8();
            stream.Seek(seek, SeekOrigin.Current);
            if (length > 255)
            {
                throw new InvalidOperationException();
            }
            return stream.ReadString(length);
        }

        public static void WriteStringU8(this Stream stream, string value)
        {
            stream.WriteStringU8(value, true);
        }

        public static void WriteStringU8(this Stream stream, string value, bool littleEndian)
        {
            byte length = (byte)value.Length;
            stream.WriteValueU8(length);
            stream.WriteString(length == value.Length ? value : value.Substring(0, length));
        }

        public static string ReadStringU16(this Stream stream, Endian endian)
        {
            var length = stream.ReadValueU16(endian);
            if (length > 0x3FF)
            {
                throw new InvalidOperationException();
            }
            return stream.ReadString(length);
        }

        public static void WriteStringU16(this Stream stream, string value, Endian endian)
        {
            ushort length = (ushort)value.Length;
            stream.WriteValueU16(length, endian);
            stream.WriteString(length == value.Length ? value : value.Substring(0, length));
        }

        public static string ReadStringU32(this Stream stream, Endian endian)
        {
            var length = stream.ReadValueU32(endian);
            if (length > 0x3FF)
            {
                throw new InvalidOperationException();
            }
            return stream.ReadString(length);
        }

        public static void WriteStringU32(this Stream stream, string value, Endian endian)
        {
            stream.WriteValueS32(value.Length, endian);
            stream.WriteString(value);
        }

        public static void WriteZeros(this Stream stream, uint count)
        {
            for (int i = 0; i < count; i++)
                stream.WriteByte(0);
        }
    }
}
