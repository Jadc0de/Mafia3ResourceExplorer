using System;

namespace FileFormats
{
    [Flags]
    public enum Options
    {
        None = 0,
        Compress = 1 << 0,
        OneBlock = 1 << 1,
    }
}
