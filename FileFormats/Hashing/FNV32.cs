using System;
using System.Text;

namespace FileFormats.Hashing
{
    public static class FNV32
    {
        public const uint Initial = 0x811C9DC5;

        public static uint Hash(string value)
        {
            return Hash(value, Encoding.GetEncoding(1252));
        }

        public static uint Hash(string value, Encoding encoding)
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

        public static uint Hash(byte[] buffer, int offset, int count)
        {
            return Hash(buffer, offset, count, Initial);
        }

        public static uint Hash(byte[] buffer, int offset, int count, uint hash)
        {
            if (buffer == null)
            {
                return hash;
            }

            /*for (int i = offset; i < offset + count; i++)
            {
                hash *= 0x1000193;
                hash ^= buffer[i];
            }*/

            unchecked
            {
                for (int i = offset; i < offset + count; i++)
                {
                    hash *= 0x1000193;
                    hash ^= buffer[i];
                }
            }

            return hash;
        }
    }
}
