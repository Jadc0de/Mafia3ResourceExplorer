using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace ResourceExplorer.Extensions
{
    //using SharpDX;
    //using SharpDX.Direct3D11;

    /// <summary>
    /// Custom Loader for DDS files
    /// </summary>
    public static class TextureUtilities
    {

        const int DDS_MAGIC = 0x20534444;// "DDS "

        [StructLayout(LayoutKind.Sequential)]
        struct DDS_PIXELFORMAT
        {
            public int size;
            public int flags;
            public int fourCC;
            public int RGBBitCount;
            public uint RBitMask;
            public uint GBitMask;
            public uint BBitMask;
            public uint ABitMask;
        };

        const int DDS_FOURCC = 0x00000004;// DDPF_FOURCC
        const int DDS_RGB = 0x00000040;// DDPF_RGB
        const int DDS_RGBA = 0x00000041;// DDPF_RGB | DDPF_ALPHAPIXELS
        const int DDS_LUMINANCE = 0x00020000;// DDPF_LUMINANCE
        const int DDS_LUMINANCEA = 0x00020001;// DDPF_LUMINANCE | DDPF_ALPHAPIXELS
        const int DDS_ALPHA = 0x00000002;// DDPF_ALPHA
        const int DDS_PAL8 = 0x00000020;// DDPF_PALETTEINDEXED8

        const int DDS_HEADER_FLAGS_TEXTURE = 0x00001007;// DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT
        const int DDS_HEADER_FLAGS_MIPMAP = 0x00020000;// DDSD_MIPMAPCOUNT
        const int DDS_HEADER_FLAGS_VOLUME = 0x00800000;// DDSD_DEPTH
        const int DDS_HEADER_FLAGS_PITCH = 0x00000008;// DDSD_PITCH
        const int DDS_HEADER_FLAGS_LINEARSIZE = 0x00080000;// DDSD_LINEARSIZE

        const int DDS_HEIGHT = 0x00000002;// DDSD_HEIGHT
        const int DDS_WIDTH = 0x00000004;// DDSD_WIDTH

        const int DDS_SURFACE_FLAGS_TEXTURE = 0x00001000;// DDSCAPS_TEXTURE
        const int DDS_SURFACE_FLAGS_MIPMAP = 0x00400008;// DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
        const int DDS_SURFACE_FLAGS_CUBEMAP = 0x00000008;// DDSCAPS_COMPLEX

        const int DDS_CUBEMAP_POSITIVEX = 0x00000600;// DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
        const int DDS_CUBEMAP_NEGATIVEX = 0x00000a00;// DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
        const int DDS_CUBEMAP_POSITIVEY = 0x00001200;// DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
        const int DDS_CUBEMAP_NEGATIVEY = 0x00002200;// DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
        const int DDS_CUBEMAP_POSITIVEZ = 0x00004200;// DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
        const int DDS_CUBEMAP_NEGATIVEZ = 0x00008200;// DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ

        const int DDS_CUBEMAP_ALLFACES = (DDS_CUBEMAP_POSITIVEX | DDS_CUBEMAP_NEGATIVEX | DDS_CUBEMAP_POSITIVEY | DDS_CUBEMAP_NEGATIVEY | DDS_CUBEMAP_POSITIVEZ | DDS_CUBEMAP_NEGATIVEZ);

        const int DDS_CUBEMAP = 0x00000200;// DDSCAPS2_CUBEMAP

        const int DDS_FLAGS_VOLUME = 0x00200000;// DDSCAPS2_VOLUME

        [StructLayout(LayoutKind.Sequential)]
        struct DDS_HEADER
        {
            public int size;
            public int flags;
            public int height;
            public int width;
            public int pitchOrLinearSize;
            public int depth; // only if DDS_HEADER_FLAGS_VOLUME is set in flags
            public int mipMapCount;
            //===11
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)]
            public int[] reserved1;

            public DDS_PIXELFORMAT ddspf;
            public int caps;
            public int caps2;
            public int caps3;
            public int caps4;
            public int reserved2;


        }

        enum D3D10_RESOURCE_DIMENSION : int
        {
            UNKNOWN = 0,
            BUFFER = 1,
            TEXTURE1D = 2,
            TEXTURE2D = 3,
            TEXTURE3D = 4
        };

        public enum Format : int
        {
            UNKNOWN = 0,
            R32G32B32A32_TYPELESS = 1,
            R32G32B32A32_FLOAT = 2,
            R32G32B32A32_UINT = 3,
            R32G32B32A32_SINT = 4,
            R32G32B32_TYPELESS = 5,
            R32G32B32_FLOAT = 6,
            R32G32B32_UINT = 7,
            R32G32B32_SINT = 8,
            R16G16B16A16_TYPELESS = 9,
            R16G16B16A16_FLOAT = 10,
            R16G16B16A16_UNORM = 11,
            R16G16B16A16_UINT = 12,
            R16G16B16A16_SNORM = 13,
            R16G16B16A16_SINT = 14,
            R32G32_TYPELESS = 15,
            R32G32_FLOAT = 16,
            R32G32_UINT = 17,
            R32G32_SINT = 18,
            R32G8X24_TYPELESS = 19,
            D32_FLOAT_S8X24_UINT = 20,
            R32_FLOAT_X8X24_TYPELESS = 21,
            X32_TYPELESS_G8X24_UINT = 22,
            R10G10B10A2_TYPELESS = 23,
            R10G10B10A2_UNORM = 24,
            R10G10B10A2_UINT = 25,
            R11G11B10_FLOAT = 26,
            R8G8B8A8_TYPELESS = 27,
            R8G8B8A8_UNORM = 28,
            R8G8B8A8_UNORM_SRGB = 29,
            R8G8B8A8_UINT = 30,
            R8G8B8A8_SNORM = 31,
            R8G8B8A8_SINT = 32,
            R16G16_TYPELESS = 33,
            R16G16_FLOAT = 34,
            R16G16_UNORM = 35,
            R16G16_UINT = 36,
            R16G16_SNORM = 37,
            R16G16_SINT = 38,
            R32_TYPELESS = 39,
            D32_FLOAT = 40,
            R32_FLOAT = 41,
            R32_UINT = 42,
            R32_SINT = 43,
            R24G8_TYPELESS = 44,
            D24_UNORM_S8_UINT = 45,
            R24_UNORM_X8_TYPELESS = 46,
            X24_TYPELESS_G8_UINT = 47,
            R8G8_TYPELESS = 48,
            R8G8_UNORM = 49,
            R8G8_UINT = 50,
            R8G8_SNORM = 51,
            R8G8_SINT = 52,
            R16_TYPELESS = 53,
            R16_FLOAT = 54,
            D16_UNORM = 55,
            R16_UNORM = 56,
            R16_UINT = 57,
            R16_SNORM = 58,
            R16_SINT = 59,
            R8_TYPELESS = 60,
            R8_UNORM = 61,
            R8_UINT = 62,
            R8_SNORM = 63,
            R8_SINT = 64,
            A8_UNORM = 65,
            R1_UNORM = 66,
            R9G9B9E5_SHAREDEXP = 67,
            R8G8_B8G8_UNORM = 68,
            G8R8_G8B8_UNORM = 69,
            BC1_TYPELESS = 70,
            BC1_UNORM = 71,
            BC1_UNORM_SRGB = 72,
            BC2_TYPELESS = 73,
            BC2_UNORM = 74,
            BC2_UNORM_SRGB = 75,
            BC3_TYPELESS = 76,
            BC3_UNORM = 77,
            BC3_UNORM_SRGB = 78,
            BC4_TYPELESS = 79,
            BC4_UNORM = 80,
            BC4_SNORM = 81,
            BC5_TYPELESS = 82,
            BC5_UNORM = 83,
            BC5_SNORM = 84,
            B5G6R5_UNORM = 85,
            B5G5R5A1_UNORM = 86,
            B8G8R8A8_UNORM = 87,
            B8G8R8X8_UNORM = 88,
            R10G10B10_XR_BIAS_A2_UNORM = 89,
            B8G8R8A8_TYPELESS = 90,
            B8G8R8A8_UNORM_SRGB = 91,
            B8G8R8X8_TYPELESS = 92,
            B8G8R8X8_UNORM_SRGB = 93,
            BC6H_TYPELESS = 94,
            BC6H_UF16 = 95,
            BC6H_SF16 = 96,
            BC7_TYPELESS = 97,
            BC7_UNORM = 98,
            BC7_UNORM_SRGB = 99,
            AYUV = 100,
            Y410 = 101,
            Y416 = 102,
            NV12 = 103,
            P010 = 104,
            P016 = 105,
            F420_OPAQUE = 106,
            YUY2 = 107,
            Y210 = 108,
            Y216 = 109,
            NV11 = 110,
            AI44 = 111,
            IA44 = 112,
            P8 = 113,
            A8P8 = 114,
            B4G4R4A4_UNORM = 115,
            P208 = 130,
            V208 = 131,
            V408 = 132,
            FORCE_UINT = -1 // 0xffffffff
        };

        [StructLayout(LayoutKind.Sequential)]
        struct DDS_HEADER_DXT10
        {
            public Format dxgiFormat;
            public int resourceDimension;
            public int miscFlag; // see D3D11_RESOURCE_MISC_FLAG
            public int arraySize;
            public int reserved;
        }

        static int BitsPerPixel(Format fmt)
        {
            switch (fmt)
            {
                case Format.R32G32B32A32_TYPELESS:
                case Format.R32G32B32A32_FLOAT:
                case Format.R32G32B32A32_UINT:
                case Format.R32G32B32A32_SINT:
                    return 128;

                case Format.R32G32B32_TYPELESS:
                case Format.R32G32B32_FLOAT:
                case Format.R32G32B32_UINT:
                case Format.R32G32B32_SINT:
                    return 96;

                case Format.R16G16B16A16_TYPELESS:
                case Format.R16G16B16A16_FLOAT:
                case Format.R16G16B16A16_UNORM:
                case Format.R16G16B16A16_UINT:
                case Format.R16G16B16A16_SNORM:
                case Format.R16G16B16A16_SINT:
                case Format.R32G32_TYPELESS:
                case Format.R32G32_FLOAT:
                case Format.R32G32_UINT:
                case Format.R32G32_SINT:
                case Format.R32G8X24_TYPELESS:
                case Format.D32_FLOAT_S8X24_UINT:
                case Format.R32_FLOAT_X8X24_TYPELESS:
                case Format.X32_TYPELESS_G8X24_UINT:
                    return 64;

                case Format.R10G10B10A2_TYPELESS:
                case Format.R10G10B10A2_UNORM:
                case Format.R10G10B10A2_UINT:
                case Format.R11G11B10_FLOAT:
                case Format.R8G8B8A8_TYPELESS:
                case Format.R8G8B8A8_UNORM:
                case Format.R8G8B8A8_UNORM_SRGB:
                case Format.R8G8B8A8_UINT:
                case Format.R8G8B8A8_SNORM:
                case Format.R8G8B8A8_SINT:
                case Format.R16G16_TYPELESS:
                case Format.R16G16_FLOAT:
                case Format.R16G16_UNORM:
                case Format.R16G16_UINT:
                case Format.R16G16_SNORM:
                case Format.R16G16_SINT:
                case Format.R32_TYPELESS:
                case Format.D32_FLOAT:
                case Format.R32_FLOAT:
                case Format.R32_UINT:
                case Format.R32_SINT:
                case Format.R24G8_TYPELESS:
                case Format.D24_UNORM_S8_UINT:
                case Format.R24_UNORM_X8_TYPELESS:
                case Format.X24_TYPELESS_G8_UINT:
                case Format.R9G9B9E5_SHAREDEXP:
                case Format.R8G8_B8G8_UNORM:
                case Format.G8R8_G8B8_UNORM:
                case Format.B8G8R8A8_UNORM:
                case Format.B8G8R8X8_UNORM:
                case Format.R10G10B10_XR_BIAS_A2_UNORM:
                case Format.B8G8R8A8_TYPELESS:
                case Format.B8G8R8A8_UNORM_SRGB:
                case Format.B8G8R8X8_TYPELESS:
                case Format.B8G8R8X8_UNORM_SRGB:
                    return 32;

                case Format.R8G8_TYPELESS:
                case Format.R8G8_UNORM:
                case Format.R8G8_UINT:
                case Format.R8G8_SNORM:
                case Format.R8G8_SINT:
                case Format.R16_TYPELESS:
                case Format.R16_FLOAT:
                case Format.D16_UNORM:
                case Format.R16_UNORM:
                case Format.R16_UINT:
                case Format.R16_SNORM:
                case Format.R16_SINT:
                case Format.B5G6R5_UNORM:
                case Format.B5G5R5A1_UNORM:
                case Format.B4G4R4A4_UNORM:
                    return 16;

                case Format.R8_TYPELESS:
                case Format.R8_UNORM:
                case Format.R8_UINT:
                case Format.R8_SNORM:
                case Format.R8_SINT:
                case Format.A8_UNORM:
                    return 8;

                case Format.R1_UNORM:
                    return 1;

                case Format.BC1_TYPELESS:
                case Format.BC1_UNORM:
                case Format.BC1_UNORM_SRGB:
                case Format.BC4_TYPELESS:
                case Format.BC4_UNORM:
                case Format.BC4_SNORM:
                    return 4;

                case Format.BC2_TYPELESS:
                case Format.BC2_UNORM:
                case Format.BC2_UNORM_SRGB:
                case Format.BC3_TYPELESS:
                case Format.BC3_UNORM:
                case Format.BC3_UNORM_SRGB:
                case Format.BC5_TYPELESS:
                case Format.BC5_UNORM:
                case Format.BC5_SNORM:
                case Format.BC6H_TYPELESS:
                case Format.BC6H_UF16:
                case Format.BC6H_SF16:
                case Format.BC7_TYPELESS:
                case Format.BC7_UNORM:
                case Format.BC7_UNORM_SRGB:
                    return 8;

                default:
                    return 0;
            }
        }

        //--------------------------------------------------------------------------------------
        // Get surface information for a particular format
        //--------------------------------------------------------------------------------------
        static void GetSurfaceInfo(int width, int height, Format fmt, out int outNumBytes, out int outRowBytes, out int outNumRows)
        {
            int numBytes = 0;
            int rowBytes = 0;
            int numRows = 0;

            bool bc = false;
            bool packed = false;
            int bcnumBytesPerBlock = 0;
            switch (fmt)
            {
                case Format.BC1_TYPELESS:
                case Format.BC1_UNORM:
                case Format.BC1_UNORM_SRGB:
                case Format.BC4_TYPELESS:
                case Format.BC4_UNORM:
                case Format.BC4_SNORM:
                    bc = true;
                    bcnumBytesPerBlock = 8;
                    break;

                case Format.BC2_TYPELESS:
                case Format.BC2_UNORM:
                case Format.BC2_UNORM_SRGB:
                case Format.BC3_TYPELESS:
                case Format.BC3_UNORM:
                case Format.BC3_UNORM_SRGB:
                case Format.BC5_TYPELESS:
                case Format.BC5_UNORM:
                case Format.BC5_SNORM:
                case Format.BC6H_TYPELESS:
                case Format.BC6H_UF16:
                case Format.BC6H_SF16:
                case Format.BC7_TYPELESS:
                case Format.BC7_UNORM:
                case Format.BC7_UNORM_SRGB:
                    bc = true;
                    bcnumBytesPerBlock = 16;
                    break;

                case Format.R8G8_B8G8_UNORM:
                case Format.G8R8_G8B8_UNORM:
                    packed = true;
                    break;
            }

            if (bc)
            {
                int numBlocksWide = 0;
                if (width > 0)
                {
                    numBlocksWide = Math.Max(1, (width + 3) / 4);
                }
                int numBlocksHigh = 0;
                if (height > 0)
                {
                    numBlocksHigh = Math.Max(1, (height + 3) / 4);
                }
                rowBytes = numBlocksWide * bcnumBytesPerBlock;
                numRows = numBlocksHigh;
            }
            else if (packed)
            {
                rowBytes = ((width + 1) >> 1) * 4;
                numRows = height;
            }
            else
            {
                int bpp = BitsPerPixel(fmt);
                rowBytes = (width * bpp + 7) / 8; // round up to nearest byte
                numRows = height;
            }

            numBytes = rowBytes * numRows;

            outNumBytes = numBytes;
            outRowBytes = rowBytes;
            outNumRows = numRows;
        }

        static bool ISBITMASK(DDS_PIXELFORMAT ddpf, uint r, uint g, uint b, uint a)
        {
            return (ddpf.RBitMask == r && ddpf.GBitMask == g && ddpf.BBitMask == b && ddpf.ABitMask == a);
        }

        static int MAKEFOURCC(int ch0, int ch1, int ch2, int ch3)
        {
            return ((int)(byte)(ch0) | ((int)(byte)(ch1) << 8) | ((int)(byte)(ch2) << 16) | ((int)(byte)(ch3) << 24));
        }


        static Format GetDXGIFormat(DDS_PIXELFORMAT ddpf)
        {

            if ((ddpf.flags & DDS_RGB) > 0)
            {
                // Note that sRGB formats are written using the "DX10" extended header

                switch (ddpf.RGBBitCount)
                {
                    case 32:
                        if (ISBITMASK(ddpf, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000))
                        {
                            return Format.R8G8B8A8_UNORM;
                        }

                        if (ISBITMASK(ddpf, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000))
                        {
                            return Format.B8G8R8A8_UNORM;
                        }

                        if (ISBITMASK(ddpf, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000))
                        {
                            return Format.B8G8R8X8_UNORM;
                        }

                        // No DXGI format maps to ISBITMASK(0x000000ff, 0x0000ff00, 0x00ff0000, 0x00000000) aka D3DFMT_X8B8G8R8

                        // Note that many common DDS reader/writers (including D3DX) swap the
                        // the RED/BLUE masks for 10:10:10:2 formats. We assumme
                        // below that the 'backwards' header mask is being used since it is most
                        // likely written by D3DX. The more robust solution is to use the 'DX10'
                        // header extension and specify the DXGI_FORMAT_R10G10B10A2_UNORM format directly

                        // For 'correct' writers, this should be 0x000003ff, 0x000ffc00, 0x3ff00000 for RGB data
                        if (ISBITMASK(ddpf, 0x3ff00000, 0x000ffc00, 0x000003ff, 0xc0000000))
                        {
                            return Format.R10G10B10A2_UNORM;
                        }

                        // No DXGI format maps to ISBITMASK(0x000003ff, 0x000ffc00, 0x3ff00000, 0xc0000000) aka D3DFMT_A2R10G10B10

                        if (ISBITMASK(ddpf, 0x0000ffff, 0xffff0000, 0x00000000, 0x00000000))
                        {
                            return Format.R16G16_UNORM;
                        }

                        if (ISBITMASK(ddpf, 0xffffffff, 0x00000000, 0x00000000, 0x00000000))
                        {
                            // Only 32-bit color channel format in D3D9 was R32F
                            return Format.R32_FLOAT; // D3DX writes this out as a FourCC of 114
                        }
                        break;

                    case 24:
                        // No 24bpp DXGI formats aka D3DFMT_R8G8B8
                        break;

                    case 16:
                        if (ISBITMASK(ddpf, 0x7c00, 0x03e0, 0x001f, 0x8000))
                        {
                            return Format.B5G5R5A1_UNORM;
                        }
                        if (ISBITMASK(ddpf, 0xf800, 0x07e0, 0x001f, 0x0000))
                        {
                            return Format.B5G6R5_UNORM;
                        }

                        // No DXGI format maps to ISBITMASK(0x7c00, 0x03e0, 0x001f, 0x0000) aka D3DFMT_X1R5G5B5
                        if (ISBITMASK(ddpf, 0x0f00, 0x00f0, 0x000f, 0xf000))
                        {
                            return Format.B4G4R4A4_UNORM;
                        }

                        // No DXGI format maps to ISBITMASK(0x0f00, 0x00f0, 0x000f, 0x0000) aka D3DFMT_X4R4G4B4

                        // No 3:3:2, 3:3:2:8, or paletted DXGI formats aka D3DFMT_A8R3G3B2, D3DFMT_R3G3B2, D3DFMT_P8, D3DFMT_A8P8, etc.
                        break;
                }
            }
            else if ((ddpf.flags & DDS_LUMINANCE) > 0)
            {
                if (8 == ddpf.RGBBitCount)
                {
                    if (ISBITMASK(ddpf, 0x000000ff, 0x00000000, 0x00000000, 0x00000000))
                    {
                        return Format.R8_UNORM; // D3DX10/11 writes this out as DX10 extension
                    }

                    // No DXGI format maps to ISBITMASK(0x0f, 0x00, 0x00, 0xf0) aka D3DFMT_A4L4
                }

                if (16 == ddpf.RGBBitCount)
                {
                    if (ISBITMASK(ddpf, 0x0000ffff, 0x00000000, 0x00000000, 0x00000000))
                    {
                        return Format.R16_UNORM; // D3DX10/11 writes this out as DX10 extension
                    }
                    if (ISBITMASK(ddpf, 0x000000ff, 0x00000000, 0x00000000, 0x0000ff00))
                    {
                        return Format.R8G8_UNORM; // D3DX10/11 writes this out as DX10 extension
                    }
                }
            }
            else if ((ddpf.flags & DDS_ALPHA) > 0)
            {
                if (8 == ddpf.RGBBitCount)
                {
                    return Format.A8_UNORM;
                }
            }
            else if ((ddpf.flags & DDS_FOURCC) > 0)
            {
                if (MAKEFOURCC('D', 'X', 'T', '1') == ddpf.fourCC)
                {
                    return Format.BC1_UNORM;
                }
                if (MAKEFOURCC('D', 'X', 'T', '3') == ddpf.fourCC)
                {
                    return Format.BC2_UNORM;
                }
                if (MAKEFOURCC('D', 'X', 'T', '5') == ddpf.fourCC)
                {
                    return Format.BC3_UNORM;
                }

                // While pre-mulitplied alpha isn't directly supported by the DXGI formats,
                // they are basically the same as these BC formats so they can be mapped
                if (MAKEFOURCC('D', 'X', 'T', '2') == ddpf.fourCC)
                {
                    return Format.BC2_UNORM;
                }
                if (MAKEFOURCC('D', 'X', 'T', '4') == ddpf.fourCC)
                {
                    return Format.BC3_UNORM;
                }

                if (MAKEFOURCC('A', 'T', 'I', '1') == ddpf.fourCC)
                {
                    return Format.BC4_UNORM;
                }
                if (MAKEFOURCC('B', 'C', '4', 'U') == ddpf.fourCC)
                {
                    return Format.BC4_UNORM;
                }
                if (MAKEFOURCC('B', 'C', '4', 'S') == ddpf.fourCC)
                {
                    return Format.BC4_SNORM;
                }

                if (MAKEFOURCC('A', 'T', 'I', '2') == ddpf.fourCC)
                {
                    return Format.BC5_UNORM;
                }
                if (MAKEFOURCC('B', 'C', '5', 'U') == ddpf.fourCC)
                {
                    return Format.BC5_UNORM;
                }
                if (MAKEFOURCC('B', 'C', '5', 'S') == ddpf.fourCC)
                {
                    return Format.BC5_SNORM;
                }

                // BC6H and BC7 are written using the "DX10" extended header

                if (MAKEFOURCC('R', 'G', 'B', 'G') == ddpf.fourCC)
                {
                    return Format.R8G8_B8G8_UNORM;
                }
                if (MAKEFOURCC('G', 'R', 'G', 'B') == ddpf.fourCC)
                {
                    return Format.G8R8_G8B8_UNORM;
                }

                // Check for D3DFORMAT enums being set here
                switch (ddpf.fourCC)
                {
                    case 36: // D3DFMT_A16B16G16R16
                        return Format.R16G16B16A16_UNORM;

                    case 110: // D3DFMT_Q16W16V16U16
                        return Format.R16G16B16A16_SNORM;

                    case 111: // D3DFMT_R16F
                        return Format.R16_FLOAT;

                    case 112: // D3DFMT_G16R16F
                        return Format.R16G16_FLOAT;

                    case 113: // D3DFMT_A16B16G16R16F
                        return Format.R16G16B16A16_FLOAT;

                    case 114: // D3DFMT_R32F
                        return Format.R32_FLOAT;

                    case 115: // D3DFMT_G32R32F
                        return Format.R32G32_FLOAT;

                    case 116: // D3DFMT_A32B32G32R32F
                        return Format.R32G32B32A32_FLOAT;
                }
            }

            return Format.UNKNOWN;
        }


        static T ByteArrayToStructure<T>(byte[] bytes, int start, int count) where T : struct
        {

            byte[] temp = bytes.Skip(start).Take(count).ToArray();
            GCHandle handle = GCHandle.Alloc(temp, GCHandleType.Pinned);
            T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
            return stuff;
        }


        public static Format TextureInfo(byte[] data)
        {
            // bool isCubeMap;

            // Validate DDS file in memory
            DDS_HEADER header = new DDS_HEADER();

            int ddsHeaderSize = Marshal.SizeOf(header);
            int ddspfSize = Marshal.SizeOf(new DDS_PIXELFORMAT());
            int ddsHeader10Size = Marshal.SizeOf(new DDS_HEADER_DXT10());

            if (data.Length < (sizeof(uint) + ddsHeaderSize))
            {
                throw new Exception();
            }

            //first is magic number
            int dwMagicNumber = BitConverter.ToInt32(data, 0);
            if (dwMagicNumber != DDS_MAGIC)
            {
                throw new Exception();
            }

            header = ByteArrayToStructure<DDS_HEADER>(data, 4, ddsHeaderSize);

            // Verify header to validate DDS file
            if (header.size != ddsHeaderSize || header.ddspf.size != ddspfSize)
            {
                throw new Exception();
            }

            // Check for DX10 extension
            bool bDXT10Header = false;
            if (((header.ddspf.flags & DDS_FOURCC) > 0) && (MAKEFOURCC('D', 'X', '1', '0') == header.ddspf.fourCC))
            {
                // Must be long enough for both headers and magic value
                if (data.Length < (ddsHeaderSize + 4 + ddsHeader10Size))
                {
                    throw new Exception();
                }

                bDXT10Header = true;
            }

            int offset = 4 + ddsHeaderSize + (bDXT10Header ? ddsHeader10Size : 0);

            // return InitTextureFromData(device, context, header, null, data, offset, 0, out isCubeMap);
            // return InitTextureFromData(device, context, header, null, data, offset, 0, out isCubeMap);
            // return InitTextureFromData(device, context, header, null, data, offset, 0, out isCubeMap);

            DDS_HEADER_DXT10? header10 = null;
            if (bDXT10Header)
            {
                header10 = ByteArrayToStructure<DDS_HEADER_DXT10>(data, 4 + ddsHeaderSize, ddsHeader10Size);
            }

            int width = header.width;
            int height = header.height;
            int depth = header.depth;

            D3D10_RESOURCE_DIMENSION resDim = D3D10_RESOURCE_DIMENSION.UNKNOWN;
            int arraySize = 1;
            Format format = Format.UNKNOWN;
            // isCubeMap = false;

            int mipCount = header.mipMapCount;
            if (0 == mipCount)
            {
                mipCount = 1;
            }

            if (((header.ddspf.flags & DDS_FOURCC) > 0) && (MAKEFOURCC('D', 'X', '1', '0') == header.ddspf.fourCC))
            {
                DDS_HEADER_DXT10 d3d10ext = header10.Value;

                arraySize = d3d10ext.arraySize;
                if (arraySize == 0)
                {
                    throw new Exception();
                }

                if (BitsPerPixel(d3d10ext.dxgiFormat) == 0)
                {
                    throw new Exception();
                }

                format = d3d10ext.dxgiFormat;

                switch ((D3D10_RESOURCE_DIMENSION)d3d10ext.resourceDimension)
                {
                    case D3D10_RESOURCE_DIMENSION.TEXTURE1D:
                        // D3DX writes 1D textures with a fixed Height of 1
                        if ((header.flags & DDS_HEIGHT) > 0 && height != 1)
                        {
                            throw new Exception();
                        }
                        height = depth = 1;
                        break;

                    case D3D10_RESOURCE_DIMENSION.TEXTURE2D:
                        //D3D11_RESOURCE_MISC_TEXTURECUBE
                        if ((d3d10ext.miscFlag & 0x4) > 0)
                        {
                            arraySize *= 6;
                            // isCubeMap = true;
                        }
                        depth = 1;
                        break;

                    case D3D10_RESOURCE_DIMENSION.TEXTURE3D:
                        if (!((header.flags & DDS_HEADER_FLAGS_VOLUME) > 0))
                        {
                            throw new Exception();
                        }

                        if (arraySize > 1)
                        {
                            throw new Exception();
                        }
                        break;

                    default:
                        throw new Exception();
                }

                resDim = (D3D10_RESOURCE_DIMENSION)d3d10ext.resourceDimension;
            }
            else
            {
                format = GetDXGIFormat(header.ddspf);

                if (format == Format.UNKNOWN)
                {
                    throw new Exception();
                }

                if ((header.flags & DDS_HEADER_FLAGS_VOLUME) > 0)
                {
                    resDim = D3D10_RESOURCE_DIMENSION.TEXTURE3D;
                }
                else
                {
                    if ((header.caps2 & DDS_CUBEMAP) > 0)
                    {
                        // We require all six faces to be defined
                        if ((header.caps2 & DDS_CUBEMAP_ALLFACES) != DDS_CUBEMAP_ALLFACES)
                        {
                            throw new Exception();
                        }

                        arraySize = 6;
                        // isCubeMap = true;
                    }

                    depth = 1;
                    resDim = D3D10_RESOURCE_DIMENSION.TEXTURE2D;
                }
            }

            return format;
        }


        /*private static List<DataBox> FillInitData(IntPtr pointer, int width, int height, int depth, int mipCount, int arraySize, Format format, int maxsize, int bitSize, int offset)
        {
            pointer += offset;

            List<DataBox> boxes = new List<DataBox>();

            int NumBytes = 0;
            int RowBytes = 0;
            int NumRows = 0;

            int index = 0;

            for (int j = 0; j < arraySize; j++)
            {
                int w = width;
                int h = height;
                int d = depth;
                for (int i = 0; i < mipCount; i++)
                {
                    GetSurfaceInfo(w, h, format, out NumBytes, out RowBytes, out NumRows);

                    DataBox box = new DataBox(pointer, RowBytes, NumBytes);

                    boxes.Add(box);
                    index++;

                    pointer += NumBytes * d;

                    w = w >> 1;
                    h = h >> 1;
                    d = d >> 1;
                    if (w == 0)
                    {
                        w = 1;
                    }
                    if (h == 0)
                    {
                        h = 1;
                    }
                    if (d == 0)
                    {
                        d = 1;
                    }
                }
            }

            return boxes;
        }*/


        /*private static ShaderResourceView InitTextureFromData(Device d3dDevice, DeviceContext context, DDS_HEADER header, DDS_HEADER_DXT10? header10, byte[] bitData, int offset, int maxsize, out bool isCubeMap)
        {
            int width = header.width;
            int height = header.height;
            int depth = header.depth;

            D3D10_RESOURCE_DIMENSION resDim = D3D10_RESOURCE_DIMENSION.UNKNOWN;
            int arraySize = 1;
            Format format = Format.UNKNOWN;
            isCubeMap = false;

            int mipCount = header.mipMapCount;
            if (0 == mipCount)
            {
                mipCount = 1;
            }

            if (((header.ddspf.flags & DDS_FOURCC) > 0) && (MAKEFOURCC('D', 'X', '1', '0') == header.ddspf.fourCC))
            {
                DDS_HEADER_DXT10 d3d10ext = header10.Value;

                arraySize = d3d10ext.arraySize;
                if (arraySize == 0)
                {
                    throw new Exception();
                }

                if (BitsPerPixel(d3d10ext.dxgiFormat) == 0)
                {
                    throw new Exception();
                }

                format = d3d10ext.dxgiFormat;

                switch ((D3D10_RESOURCE_DIMENSION)d3d10ext.resourceDimension)
                {
                    case D3D10_RESOURCE_DIMENSION.TEXTURE1D:
                        // D3DX writes 1D textures with a fixed Height of 1
                        if ((header.flags & DDS_HEIGHT) > 0 && height != 1)
                        {
                            throw new Exception();
                        }
                        height = depth = 1;
                        break;

                    case D3D10_RESOURCE_DIMENSION.TEXTURE2D:
                        //D3D11_RESOURCE_MISC_TEXTURECUBE
                        if ((d3d10ext.miscFlag & 0x4) > 0)
                        {
                            arraySize *= 6;
                            isCubeMap = true;
                        }
                        depth = 1;
                        break;

                    case D3D10_RESOURCE_DIMENSION.TEXTURE3D:
                        if (!((header.flags & DDS_HEADER_FLAGS_VOLUME) > 0))
                        {
                            throw new Exception();
                        }

                        if (arraySize > 1)
                        {
                            throw new Exception();
                        }
                        break;

                    default:
                        throw new Exception();
                }

                resDim = (D3D10_RESOURCE_DIMENSION)d3d10ext.resourceDimension;
            }
            else
            {
                format = GetDXGIFormat(header.ddspf);

                if (format == Format.UNKNOWN)
                {
                    throw new Exception();
                }

                if ((header.flags & DDS_HEADER_FLAGS_VOLUME) > 0)
                {
                    resDim = D3D10_RESOURCE_DIMENSION.TEXTURE3D;
                }
                else
                {
                    if ((header.caps2 & DDS_CUBEMAP) > 0)
                    {
                        // We require all six faces to be defined
                        if ((header.caps2 & DDS_CUBEMAP_ALLFACES) != DDS_CUBEMAP_ALLFACES)
                        {
                            throw new Exception();
                        }

                        arraySize = 6;
                        isCubeMap = true;
                    }

                    depth = 1;
                    resDim = D3D10_RESOURCE_DIMENSION.TEXTURE2D;
                }
            }

            Resource resource = null;


            GCHandle pinnedArray = GCHandle.Alloc(bitData, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            var boxes = FillInitData(pointer, width, height, depth, mipCount, arraySize, format, 0, 0, offset);


            switch (resDim)
            {
                case ResourceDimension.Unknown:
                    break;
                case ResourceDimension.Buffer:
                    break;
                case ResourceDimension.Texture1D:
                    resource = new Texture1D(d3dDevice, new Texture1DDescription()
                    {
                        BindFlags = BindFlags.ShaderResource,
                        Format = format,
                        ArraySize = arraySize,
                        Width = width,
                        CpuAccessFlags = CpuAccessFlags.None,
                        MipLevels = mipCount,
                        OptionFlags = ResourceOptionFlags.None,
                        Usage = ResourceUsage.Default,
                    }, boxes.ToArray());
                    break;
                case ResourceDimension.Texture2D:
                    resource = new Texture2D(d3dDevice, new Texture2DDescription()
                    {
                        ArraySize = arraySize,
                        BindFlags = BindFlags.ShaderResource,
                        Format = format,
                        Height = height,
                        Width = width,
                        CpuAccessFlags = CpuAccessFlags.None,
                        MipLevels = mipCount,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = ResourceUsage.Default

                    }, boxes.ToArray());
                    break;
                case ResourceDimension.Texture3D:
                    resource = new Texture3D(d3dDevice, new Texture3DDescription()
                    {
                        Depth = depth,
                        BindFlags = BindFlags.ShaderResource,
                        Format = format,
                        Height = height,
                        Width = width,
                        CpuAccessFlags = CpuAccessFlags.None,
                        MipLevels = mipCount,
                        OptionFlags = ResourceOptionFlags.None,
                        Usage = ResourceUsage.Default

                    }, boxes.ToArray());
                    break;
                default:
                    break;
            }
            pinnedArray.Free();


            var resourceView = new ShaderResourceView(d3dDevice, resource);

            return resourceView;
        }*/


        /*private static ShaderResourceView CreateTextureFromDDS(Device device, DeviceContext context, byte[] data, out bool isCubeMap)
        {
            // Validate DDS file in memory
            DDS_HEADER header = new DDS_HEADER();

            int ddsHeaderSize = Marshal.SizeOf(header);
            int ddspfSize = Marshal.SizeOf(new DDS_PIXELFORMAT());
            int ddsHeader10Size = Marshal.SizeOf(new DDS_HEADER_DXT10());

            if (data.Length < (sizeof(uint) + ddsHeaderSize))
            {
                throw new Exception();
            }

            //first is magic number
            int dwMagicNumber = BitConverter.ToInt32(data, 0);
            if (dwMagicNumber != DDS_MAGIC)
            {
                throw new Exception();
            }

            header = ByteArrayToStructure<DDS_HEADER>(data, 4, ddsHeaderSize);

            // Verify header to validate DDS file
            if (header.size != ddsHeaderSize ||
                header.ddspf.size != ddspfSize)
            {
                throw new Exception();
            }

            // Check for DX10 extension
            bool bDXT10Header = false;
            if (((header.ddspf.flags & DDS_FOURCC) > 0) && (MAKEFOURCC('D', 'X', '1', '0') == header.ddspf.fourCC))
            {
                // Must be long enough for both headers and magic value
                if (data.Length < (ddsHeaderSize + 4 + ddsHeader10Size))
                {
                    throw new Exception();
                }

                bDXT10Header = true;
            }

            int offset = 4 + ddsHeaderSize + (bDXT10Header ? ddsHeader10Size : 0);

            return InitTextureFromData(device, context, header, null, data, offset, 0, out isCubeMap);
        }*/

        /*private static ShaderResourceView CreateTextureFromBitmap(Device device, DeviceContext context, string filename)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(filename);

            int width = bitmap.Width;
            int height = bitmap.Height;

            // Describe and create a Texture2D.
            Texture2DDescription textureDesc = new Texture2DDescription()
            {
                MipLevels = 1,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0)
            };

            System.Drawing.Imaging.BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            DataRectangle dataRectangle = new DataRectangle(data.Scan0, data.Stride);
            var buffer = new Texture2D(device, textureDesc, dataRectangle);
            bitmap.UnlockBits(data);


            var resourceView = new ShaderResourceView(device, buffer);
            buffer.Dispose();

            return resourceView;
        }*/

        /// <summary>
        /// Load texture from file
        /// </summary>
        /// <param name="device">Device</param>
        /// <param name="filename">Filename</param>
        /// <returns>Shader Resource View</returns>
        /*public static ShaderResourceView LoadTextureFromFile(this SharpDevice device, string filename)
        {
            string ext = System.IO.Path.GetExtension(filename);

            if (ext.ToLower() == ".dds")
            {
                bool isCube;
                return CreateTextureFromDDS(device.Device, device.DeviceContext, System.IO.File.ReadAllBytes(filename), out isCube);
            }
            else
            {
                return CreateTextureFromBitmap(device.Device, device.DeviceContext, filename);
            }
        }*/
    }
}
