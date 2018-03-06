using System;
using System.Text;

namespace FileFormats.Hashing
{
    public static class FNV64
    {
        public const ulong Initial = 0xCBF29CE484222325;

        public static ulong Hash(string value)
        {
            return Hash(value, Encoding.GetEncoding(1252));
        }

        public static ulong Hash(string value, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            if (value == null)
            {
                return Hash(null, 0, 0);
            }

            var bytes = encoding.GetBytes(value);
            return Hash(bytes, 0, bytes.Length);
        }

        public static ulong Hash(byte[] buffer, int offset, int count)
        {
            return Hash(buffer, offset, count, Initial);
        }

        public static ulong Hash(byte[] buffer, int offset, int count, ulong hash)
        {
            if (buffer == null)
            {
                return hash;
            }

            /*for (int i = offset; i < offset + count; i++)
            {
                hash *= 0x00000100000001B3;
                hash ^= buffer[i];
            }*/

            unchecked
            {
                for (int i = offset; i < offset + count; i++)
                {
                    hash *= 0x00000100000001B3;
                    hash ^= buffer[i];
                }
            }

            return hash;
        }

    }
}
