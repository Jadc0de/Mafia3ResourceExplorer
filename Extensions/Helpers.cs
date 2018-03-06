using System;
using System.IO;
using Gibbed.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms;

namespace ResourceExplorer
{
    public static partial class Helpers
    {
        public static void ErrorLog(Exception ex)
        {
            string path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "error.txt");
            if (IsFileLocked(new FileInfo(path)) == false)
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    sw.WriteLine("=========== Error =========== " + DateTime.Now);
                    sw.WriteLine("Message: " + ex.Message);
                    sw.WriteLine("Stack Trace: " + ex.StackTrace);
                    sw.WriteLine("============================= " + DateTime.Now);
                }
            }
        }

        public static DialogResult ShowError(Exception ex)
        {
            return ShowError(ex.Message + "\r\n" + ex.StackTrace);
        }

        internal static DialogResult ShowError(string text, string message = "Error")
        {
            DialogResult result = FlexibleMessageBox.Show(text + "\r\n", message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return result;
        }

        internal static DialogResult ShowInformation(string text, string message = "Message")
        {
            DialogResult result = FlexibleMessageBox.Show(text + "\r\n", message, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return result;
        }

        internal static DialogResult ShowAsterisk(string text, string message = "Message")
        {
            DialogResult result = FlexibleMessageBox.Show(text + "\r\n", message, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            return result;
        }

        public static string BytesToString(long in_bytes)
		{
			if (in_bytes < 1024L)
			{
				return string.Format("{0} bytes", in_bytes);
			}
			if (in_bytes < 1048576L)
			{
				return string.Format("{0:F1} KB", (float)in_bytes / 1024f);
			}
			if (in_bytes < 1073741824L)
			{
				return string.Format("{0:F1} MB", (float)in_bytes / 1048576f);
			}
			if (in_bytes < 1099511627776L)
			{
				return string.Format("{0:F1} GB", (float)in_bytes / 1.07374182E+09f);
			}
			return string.Format("{0:F1} TB", (float)in_bytes / 1.09951163E+12f);
		}
		
        public static T BytesToStruct<T>(byte[] bytes) where T : struct
        {
            var structSize = Marshal.SizeOf(typeof(T));
            var pointer = IntPtr.Zero;
            try
            {
                pointer = Marshal.AllocHGlobal(structSize);
                Marshal.Copy(bytes, 0, pointer, structSize);
                return (T)Marshal.PtrToStructure(pointer, typeof(T));
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(pointer);
            }
        }

        public static T BytesToStruct<T>(byte[] bytes, int startIndex) where T : struct
        {
            var structSize = Marshal.SizeOf(typeof(T));
            var pointer = IntPtr.Zero;
            try
            {
                pointer = Marshal.AllocHGlobal(structSize);
                Marshal.Copy(bytes, startIndex, pointer, structSize);
                return (T)Marshal.PtrToStructure(pointer, typeof(T));
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(pointer);
            }
        }

        public static byte[] StructToBytes<T>(T structObject) where T : struct
        {
            var structSize = Marshal.SizeOf(typeof(T));
            var bytes = new byte[structSize];
            var pointer = IntPtr.Zero;
            try
            {
                pointer = Marshal.AllocHGlobal(structSize);
                Marshal.StructureToPtr(structObject, pointer, true);
                Marshal.Copy(pointer, bytes, 0, structSize);
                return bytes;
            }
            finally
            {
                if (pointer != IntPtr.Zero)
                    Marshal.FreeHGlobal(pointer);
            }
        }

        /// <summary>
        /// Reads a byte buffer from unmanaged memory.
        /// </summary>
        /// <param name="pointer">Pointer to unmanaged memory</param>
        /// <param name="numBytes">Number of bytes to read</param>
        /// <returns>Byte buffer, or null if the pointer was no valid</returns>
        public static byte[] ReadByteBuffer(IntPtr pointer, int numBytes)
        {
            if (pointer == IntPtr.Zero)
                return null;

            byte[] bytes = new byte[numBytes];
            Marshal.Copy(pointer, bytes, 0, numBytes);
            return bytes;
        }

        /// <summary>
        /// Convienence method for marshaling a pointer to a structure.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="ptr">Pointer to marshal</param>
        /// <returns>Marshaled structure</returns>
        public static T MarshalStructure<T>(IntPtr ptr) where T : struct
        {
            if (ptr == IntPtr.Zero)
            {
                return default(T);
            }
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        public static void CleanUp()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public static bool IsFileLocked(FileInfo file)
        {
            if (!file.Exists)
            {
                return false;
            }

            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
#if DEBUG
            catch (IOException ex)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)

                ShowError(ex);

                if (stream != null)
                    stream.Close();

                return true;
            }
#endif
            catch
            {
                if (stream != null)
                    stream.Close();

                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public static ulong HexLiteral2Unsigned(string hex)
        {
            if (string.IsNullOrEmpty(hex)) throw new ArgumentException("hex");

            int i = hex.Length > 1 && hex[0] == '0' && (hex[1] == 'x' || hex[1] == 'X') ? 2 : 0;
            ulong value = 0;

            while (i < hex.Length)
            {
                uint x = hex[i++];

                if (x >= '0' && x <= '9') x = x - '0';
                else if (x >= 'A' && x <= 'F') x = (x - 'A') + 10;
                else if (x >= 'a' && x <= 'f') x = (x - 'a') + 10;
                else throw new ArgumentOutOfRangeException("hex");

                value = 16 * value + x;

            }

            return value;
        }

        public static bool IsHexString(string s)
        {
            // For C-style hex notation (0xFF) you can use @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z"
            // C# @"\A\b[0-9a-fA-F]+\b\Z"
            return System.Text.RegularExpressions.Regex.IsMatch(s, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");
        }

        public static string ExtensionsDetect(byte[] guess, int read)
        {
            if (read == 0)
            {
                return "null";
            }

            // DDS
            // 0x20534444
            if (read >= 4)
            {
                var dds = BitConverter.ToUInt32(guess, 0);

                if (dds == 0x20534444 || dds.Swap() == 0x44445320)
                {
                    return "dds";
                }
            }

            // Havok file HKX
            // 57E0E057 10C0C010
            if (read >= 4)
            {
                var dds = BitConverter.ToUInt32(guess, 0);

                if (dds == 0x57E0E057 || dds.Swap() == 0x57E0E057)
                {
                    return "hkx";
                }
            }

            if (
                read >= 4 &&
                guess[0] == 0x89 &&
                guess[1] == 'P' &&
                guess[2] == 'N' &&
                guess[3] == 'G')
            {
                return "png";
            }
            else if (
                read >= 4 &&
                guess[0] == 'F' &&
                guess[1] == 'S' &&
                guess[2] == 'B' &&
                guess[3] == '4')
            {
                return "sam";
            }
            else if (
                read >= 4 &&
                guess[0] == 'M' &&
                guess[1] == 'u' &&
                guess[2] == 's')
            {
                return "mus";
            }
            else if (
                read >= 4 &&
                guess[0] == 0x21 &&
                guess[1] == 'W' &&
                guess[2] == 'A' &&
                guess[3] == 'R')
            {
                return "raw";
            }

            return "bin";
        }
    }
}