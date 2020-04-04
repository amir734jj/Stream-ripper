using System;
using System.IO;
using Microsoft.Extensions.Logging;
using StreamRipper.Interfaces;
using StreamRipper.Models.Events;
using StreamRipper.Models.Song;
using StreamRipper.Models.State;

namespace StreamRipper.Logic
{
    public static class EventHandlerImpl
    {
        public static readonly Action<EventState, MetadataChangedEventArg> MetadataChangedHandler = (state, arg) =>
        {
            state.Logger.LogTrace("MetadataChangedEventHandler invoked", arg);
            
            // if count is greater than zero, ignore the first one
            if (state.Count <= 0) return;

            state.EventHandlers.SongChangedEventHandlers.Invoke(state, new SongChangedEventArg
            {
                // Clone SongInfo
                SongInfo = (SongInfo) state.SongInfo.Clone()
            });

            // Clean the buffer
            state.SongInfo.Dispose();
        };
        
        public static readonly Action<EventState, StreamUpdateEventArg> StreamUpdateEventHandler = async (state, arg) =>
        {
            state.Logger.LogTrace("StreamUpdateEventHandler invoked", arg);
              
            // Append to MemoryStream
            await state.SongInfo.Stream.WriteAsync(arg.SongRawPartial, 0, arg.SongRawPartial.Length);
        };
        
        public static readonly Action<EventState, StreamStartedEventArg> StreamStartedEventHandler = (state, arg) =>
        {
            state.Logger.LogTrace("StreamStartedEventHandler invoked", arg);
            
            // Initialize the SongInfo
            state.SongInfo = new SongInfo
            {
                // Empty properties
                SongMetadata = new SongMetadata(),
                Stream = new MemoryStream()
            };
        };
        
        public static readonly Action<EventState, StreamEndedEventArg> StreamEndedEventHandler = (state, arg) =>
        {
            state.Logger.LogTrace("StreamEndedEventHandler invoked", arg);
        };
        
        public static readonly Action<EventState, SongChangedEventArg> SongChangedEventHandler = (state, arg) =>
        {
            state.Logger.LogTrace("SongChangedEventHandler invoked", arg);
        };
        
        public static readonly Action<EventState, StreamFailedEventArg> StreamFailedEventHandler = (state, arg) =>
        {
            state.Logger.LogTrace("StreamFailedEventHandler invoked", arg);
        };

        public static EventHandler<T> TypedHandler<T>(Action<EventState, T> handler) where T: IEvent
        {
            void Handler(object untypedEvent, T eventArg)
            {
                switch (untypedEvent)
                {
                    case EventState eventState:
                        handler(eventState, eventArg);
                        break;
                }
            }

            return Handler;
        } 
    }
}