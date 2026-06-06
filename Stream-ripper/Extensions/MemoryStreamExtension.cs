using System.IO;

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
        public static MemoryStream Clone(this MemoryStream source)
        {
            source.Seek(0, SeekOrigin.Begin);

            var destination = new MemoryStream();
            
            source.CopyTo(destination);
            
            return destination;
        }
    }
}
