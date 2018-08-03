using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
            byte[] buffer = source.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            source.Position = 0;
            source.SetLength(0);
        }

        /// <summary>
        /// Clone the memory stream
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static MemoryStream Clone(this MemoryStream source)
        {
            var pos = source.Position;
            var destination = new MemoryStream();
            source.CopyTo(destination);
            source.Position = pos;
            destination.Position = pos;
            return destination;
        }
    }
}
