using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StreamRipper.Extensions
{
    public static class MemoryStreamExtension
    {
        /// <summary>
        /// Clean the contents of memory stream
        /// </summary>
        /// <param name="source"></param>
        public static void Clear(this MemoryStream source)
        {
            source.SetLength(0);
        }

        /// <summary>
        /// Clone the memory stream
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static async Task<MemoryStream> Clone(this MemoryStream source)
        {
            source.Seek(0, SeekOrigin.Begin);
            var destination = new MemoryStream();
            await source.CopyToAsync(destination);
            return destination;
        }

        /// <summary>
        /// Copy stream to file
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task ToFileStream(this MemoryStream source, string path)
        {
            var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

            // Needed
            source.Seek(0, SeekOrigin.Begin);
            
            // Copy to file stream
            await source.CopyToAsync(fileStream);
            
            // Release
            fileStream.Dispose();;
        }
    }
}
