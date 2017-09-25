using System;
using System.IO;

namespace SmartArch.Web.Helpers
{
    /// <summary>
    /// Represents helper for getting MIME type.
    /// </summary>
    public static class MimeType
    {
        /// <summary>
        /// Gets the MIME type from file by filename.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The MIME type of file.</returns>
        public static string Get(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentException("Can't get MIME type from file if filename is null", "filename");
            }

            var fileExtension = Path.GetExtension(filename);
            if (fileExtension == null)
            {
                throw new ArgumentException("Can't get MIME type from file if filename hasn't file extension", "filename");
            }

            string mime = "application/octetstream";
            Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileExtension.ToLower());
            if (registryKey != null && registryKey.GetValue("Content Type") != null)
            {
                mime = registryKey.GetValue("Content Type").ToString();
            }

            return mime;
        }
    }
}