using System;
using System.ComponentModel.DataAnnotations;

namespace StreamRipper.Models
{
    public class StreamRipperOptions
    {
        [Required]
        public Uri Url { get; set; }

        /// <summary>
        ///     Length of stream in bytes
        /// </summary>
        public int MaxBufferSize { get; set; } = int.MaxValue;

        /// <summary>
        ///     Validate stream if valid or not
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public StreamRipperOptions Validate()
        {
            if (Url == null)
            {
                throw new Exception("Url cannot be null!");
            }

            return this;
        }
    }
}