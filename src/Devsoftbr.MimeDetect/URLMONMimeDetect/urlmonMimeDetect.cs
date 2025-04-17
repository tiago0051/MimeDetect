using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

/// <summary>
/// Created By: Jeremy Thompson
/// Created Date: 15/03/2010
/// Description: Detect file format through URLMon, some file formats like text/plain dont get detected since they dont have any magic bytes, so use URLMon instead for these files
/// </summary>
/// <remarks></remarks>

namespace Devsoftbr.MimeDetect.URLMONMimeDetect
{
    public class urlmonMimeDetect
    {
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            IntPtr pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out IntPtr ppwzMimeOut,
            System.UInt32 dwReserverd
        );

        public string GetMimeFromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " not found");

            byte[] buffer = new byte[256];
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                if (fs.Length >= 256)
                    fs.Read(buffer, 0, 256);
                else
                    fs.Read(buffer, 0, (int)fs.Length);
            }
            try
            {
                IntPtr mimeTypePtr;
                FindMimeFromData(IntPtr.Zero, null, buffer, 256, null, 0, out mimeTypePtr, 0);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch (Exception)
            {
                return "unknown/unknown";
            }
        }

    }
}
