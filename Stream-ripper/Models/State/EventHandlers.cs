using System;
using StreamRipper.Models.Events;

namespace StreamRipper.Models.State
{
    public class EventHandlers
    {
        /// <summary>
        /// Metadata updated event handlers
        /// </summary>
        public EventHandler<MetadataChangedEventArg> MetadataChangedHandlers { get; set;}

        /// <summary>
        /// Stream updated event handlers
        /// </summary>
        public EventHandler<StreamUpdateEventArg> StreamUpdateEventHandlers { get; set; }
        
        /// <summary>
        /// Stream started event handlers
        /// </summary>
        public EventHandler<StreamStartedEventArg> StreamStartedEventHandlers { get; set; }
        
        /// <summary>
        /// Stream ended event handlers
        /// </summary>
        public EventHandler<StreamEndedEventArg> StreamEndedEventHandlers { get; set; }
        
        /// <summary>
        /// Song change event handlers
        /// </summary>
        public EventHandler<SongChangedEventArg> SongChangedEventHandlers { get; set; }

        /// <summary>
        /// Exception handler
        /// </summary>
        public EventHandler<StreamFailedEventArg> StreamFailedHandlers { get; set; }
    }
}