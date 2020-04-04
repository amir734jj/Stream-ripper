using System;
using StreamRipper.Interfaces;

namespace StreamRipper.Models.Events
{
    public class StreamFailedEventArg : IEvent
    {
        public string Message { get; set; }
        
        public Exception Exception { get; set; }
    }
}