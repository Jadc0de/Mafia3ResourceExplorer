////////////////////////////////////////////////////////////////////////
//
// This file is part of pdn-ddsfiletype-plus, a DDS FileType plugin
// for Paint.NET that adds support for the DX10 and later formats.
//
// Copyright (c) 2017 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

//using PaintDotNet;
//using PaintDotNet.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DdsFile
{
    public static class DdsFile
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DDSLoadInfo : IEquatable<DDSLoadInfo>
        {
            public int width;
            public int height;
            public int stride;
            public IntPtr scan0;

            public bool Equals(DDSLoadInfo other)
            {
                return width == other.width && height == other.height && stride == other.stride && scan0 == other.scan0;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj) == true)
                {
                    return false;
                }
                return obj is DDSLoadInfo && Equals((DDSLoadInfo)obj) == true;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = (int)width;
                    hashCode = (hashCode * 397) ^ (int)height;
                    hashCode = (hashCode * 397) ^ (int)stride;
                    hashCode = (hashCode * 397) ^ scan0.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(DDSLoadInfo left, DDSLoadInfo right)
            {
                return left.Equals(right) == true;
            }

            public static bool operator !=(DDSLoadInfo left, DDSLoadInfo right)
            {
                return left.Equals(right) == false;
            }
        }

        private const string DLL = "dxt.dll";

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DdsWriteImageCallback(IntPtr image, UIntPtr imageSize);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate void DdsProgressCallback(UIntPtr done, UIntPtr total);

        private static class DdsIO_x86
        {
            [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
            internal static unsafe extern int Load([In] byte* input, [In] UIntPtr inputSize, [In, Out] ref DDSLoadInfo info);

            [DllImport(DLL, CallingConvention = CallingConvention.StdCall)]
            internal static extern void Free([In, Out] ref DDSLoadInfo info);
        }

        private static class HResult
        {
            public const int NotSupported = unchecked((int)0x80070032); // HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED)
            public const int InvalidData = unchecked((int)0x8007000D); // HRESULT_FROM_WIN32(ERROR_INVALID_DATA)
            // 0x80004002
        }

        private static bool FAILED(int hr)
        {
            return hr < 0;
        }

        public static unsafe BGRA* GetRowAddressUnchecked(this System.Drawing.Imaging.BitmapData surf, int y)
        {
            BGRA* dstPtr = (BGRA*)surf.Scan0;

            dstPtr += y * surf.Width;

            return dstPtr;
        }

        public static unsafe System.Drawing.Bitmap Load(Stream input)
        {
            DDSLoadInfo info = new DDSLoadInfo();

            LoadDdsFile(input, ref info);

            if (info == default(DDSLoadInfo))
            {
                return null;
            }

            var bitmap = new System.Drawing.Bitmap(info.width, info.height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var rect = new System.Drawing.Rectangle(0, 0, info.width, info.height);

            var data = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                for (int y = 0; y < info.height; y++)
                {
                    byte* src = (byte*)info.scan0 + (y * info.stride);

                    BGRA* dst = data.GetRowAddressUnchecked(y);

                    for (int x = 0; x < info.width; x++)
                    {
                        dst->R = src[0];
                        dst->G = src[1];
                        dst->B = src[2];
                        dst->A = src[3];

                        src += 4;
                        dst++;
                    }
                }
            }
            finally
            {
                Free(ref info);
            }

            bitmap.UnlockBits(data);

            return bitmap;
        }

        private static unsafe void LoadDdsFile(Stream stream, ref DDSLoadInfo info)
        {
            byte[] buffer = new byte[stream.Length];
            // stream.ProperRead(buffer, 0, buffer.Length);
            stream.Read(buffer, 0, buffer.Length);

            int hr;

            fixed (byte* pBytes = buffer)
            {
                hr = DdsIO_x86.Load(pBytes, new UIntPtr((ulong)buffer.Length), ref info);
            }

            if (FAILED(hr))
            {
                switch (hr)
                {
                case HResult.InvalidData:
                    throw new FormatException("The DDS file is invalid.");
                case HResult.NotSupported:
                    throw new FormatException("The file is not a supported DDS format.");
                default:
                    Marshal.ThrowExceptionForHR(hr);
                    break;
                }
            }
        }

        private static void Free(ref DDSLoadInfo info)
        {
            DdsIO_x86.Free(ref info);
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct BGRA
    {
        [FieldOffset(0)]
        public byte B;

        [FieldOffset(1)]
        public byte G;

        [FieldOffset(2)]
        public byte R;

        [FieldOffset(3)]
        public byte A;

        /// <summary>
        /// Lets you change B, G, R, and A at the same time.
        /// </summary>
        [NonSerialized]
        [FieldOffset(0)]
        public uint Bgra;

        public const int BlueChannel = 0;
        public const int GreenChannel = 1;
        public const int RedChannel = 2;
        public const int AlphaChannel = 3;

        public const int SizeOf = 4;
    };

    public enum DdsFileFormat
    {
        BC1,
        BC2,
        BC3,
        BC4,
        BC5,
        BC6H,
        BC7,
        B8G8R8A8,
        B8G8R8X8,
        R8G8B8A8,
        B5G5R5A1,
        B4G4R4A4,
        B5G6R5
    };
}
