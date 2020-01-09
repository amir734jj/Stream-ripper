using System;

namespace StreamRipper.Models.Events
{
    public class StreamFailedEventArg
    {
        public Exception Exception { get; set; }
    }
}