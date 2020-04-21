using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace StreamRipper.Models
{
    public class StreamRipperOptions
    {
        [Required]
        public Uri Url { get; set; }

        public ILogger Logger { get; set; } = NullLogger.Instance;

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