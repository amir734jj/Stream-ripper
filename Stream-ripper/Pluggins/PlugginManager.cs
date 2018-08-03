using System;
using System.IO;
using StreamRipper.Builders;
using StreamRipper.Interfaces;
using StreamRipper.Models;
using StreamRipper.Models.Events;

namespace StreamRipper.Pluggins
{
    public class PluginManager : IPluginManager
    {
        public Action<MetadataChangedEventArg> OnMetadataChanged { get; }

        public Action<StreamUpdateEventArg> OnStreamUpdate { get; }
        
        public Action<StreamStartedEventArg> OnStreamStarted { get; }
        
        public Action<StreamEndedEventArg> OnStreamEnded { get; }
        
        public Action<SongChangedEventArg> OnSongChanged { get; }
         
        private SongInfo _songInfo;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onMetadataChanged"></param>
        /// <param name="onStreamUpdate"></param>
        /// <param name="onStreamStarted"></param>
        /// <param name="onStreamEnded"></param>
        /// <param name="onSongChanged"></param>
        public PluginManager(Action<MetadataChangedEventArg> onMetadataChanged,
            Action<StreamUpdateEventArg> onStreamUpdate, Action<StreamStartedEventArg> onStreamStarted,
            Action<StreamEndedEventArg> onStreamEnded, Action<SongChangedEventArg> onSongChanged)
        {
            // Count of songs so-far
            var count = 0;
            
            // On song changed event handler wrapper
            //     - before: set the stream seeker to the beginning
            //     - after: clear the stream
            OnSongChanged = ActionEventHandlerBuilder<SongChangedEventArg>.New()
                .AddBeforeExecution(x =>
                {
                    // Set the stream seeker to the beginning
                    x.SongInfo.Stream.Seek(0, SeekOrigin.Begin);
                })
                .SetActionHandler(onSongChanged)
                .AddAfterExecution(_ =>
                {
                    // Dispose the SongInfo
                    _songInfo.Dispose();
                })
                .WrapAsync()
                .Build();
            
            // On metadata changed event handler wrapper
            //     - before: 
            OnMetadataChanged = ActionEventHandlerBuilder<MetadataChangedEventArg>.New()
                .SetActionHandler(onMetadataChanged)
                .AddFilterExecution(x =>
                {
                    // Do not invoke event handler when count == 0
                    // ReSharper disable once ConvertToLambdaExpression
                    return count > 0;
                })
                // Trigger song change event
                .AddBeforeExecution(_ =>
                {
                    // if count is greater than zero, ignore the first one
                    if (count > 0)
                    {
                        OnSongChanged(new SongChangedEventArg {
                            // Clone SongInfo
                            SongInfo = (SongInfo) _songInfo.Clone()
                        });
                    }
                })
                // Hold on to the metadata
                .AddAfterExecution(x =>
                {
                    // Set the metadata
                    _songInfo.SongMetadata = x.SongMetadata;
                    
                    // Increment the count
                    count++;
                })
                .WrapAsync()
                .Build();

            // On stream updated event handler wrapper
            //    - after: update the stream
            OnStreamUpdate = ActionEventHandlerBuilder<StreamUpdateEventArg>.New()
                .SetActionHandler(onStreamUpdate)
                // Update the stream
                .AddAfterExecution(x =>
                {
                    // Append to MemoryStream
                    _songInfo.Stream.Write(x.SongRawPartial, 0, x.SongRawPartial.Length);
                })
                .WrapAsync()
                .Build();
            
            // On stream started event handler wrapper, initialize the fields
            //    - after: initialize the buffer
            OnStreamStarted = ActionEventHandlerBuilder<StreamStartedEventArg>.New()
                .SetActionHandler(onStreamStarted)
                .WrapAsync()
                // Initialize the buffer
                .AddAfterExecution(_ =>
                {
                    _songInfo = new SongInfo
                    {
                        // Empty properties
                        SongMetadata = new SongMetadata(),
                        Stream = new MemoryStream()
                    };
                })
                .Build();

            // On stream ended event handler wrapper
            OnStreamEnded = ActionEventHandlerBuilder<StreamEndedEventArg>.New()
                .SetActionHandler(onStreamEnded)
                .WrapAsync()
                .Build();
        }
    }
}