using System;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading.Tasks;

namespace TriExplorer.Utils
{
    static class FileHelper
    {
        #region File size prettyprint
        public static string ToFileSize(float size)
        {
            if (size == 0) return "N/A";

            string[] suffixes = { "bytes", "KB", "MB", "GB",
                "TB", "PB", "EB", "ZB", "YB"};

            for (int i = 0; i < suffixes.Length; i++)
            {
                if (size <= (Math.Pow(1024, i + 1)))
                {
                    return ThreeNonZeroDigits(size /
                        Math.Pow(1024, i)) +
                        " " + suffixes[i];
                }
            }

            return ThreeNonZeroDigits(size /
                Math.Pow(1024, suffixes.Length - 1)) +
                " " + suffixes[suffixes.Length - 1];
        }

        private static string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return value.ToString("0,0");
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return value.ToString("0.0");
            }
            else
            {
                // Two digits after the decimal.
                return value.ToString("0.00");
            }
        }
        #endregion

        #region File type description from system
        public static string GetFileTypeDescription(string fileNameOrExtension)
        {
            if (IntPtr.Zero != SHGetFileInfo(
                                fileNameOrExtension,
                                FILE_ATTRIBUTE_NORMAL,
                                out SHFILEINFO shfi,
                                (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
                                SHGFI_USEFILEATTRIBUTES | SHGFI_TYPENAME))
            {
                return shfi.szTypeName;
            }
            return null;
        }

        [DllImport("shell32")]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint SHGFI_TYPENAME = 0x000000400;     // get type name
        private const uint SHGFI_USEFILEATTRIBUTES = 0x000000010;     // use passed dwFileAttribute
        #endregion

        public async static Task<bool> CompareMd5(string filePath, string expected)
        {
            if (!File.Exists(filePath)) return false;

            return await Task.Run(() =>
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        return expected.ToUpper().Equals(
                            BitConverter.ToString(md5.ComputeHash(stream))
                                .Replace("-", ""));
                    }
                }
            });
        }
    }
}
