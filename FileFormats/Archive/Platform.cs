namespace FileFormats.Archive
{
    public enum Platform : uint
    {
        // ReSharper disable InconsistentNaming
        PC = 0x50430000, // 'PC\0\0'
        Xbox360 = 0x58424F58, // 'XBOX'
        PS3 = 0x50533300, // 'PS3\0'
        // ReSharper restore InconsistentNaming
    }
}
