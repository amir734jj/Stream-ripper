using System;
using System.IO;
using System.Linq;
using StreamRipper.Builders;
using StreamRipper.Extensions;
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
            OnSongChanged = ActionEventHandlerBuilder<SongChangedEventArg>.New()
                .AddFilterExecution(x =>
                {
                    // Do not invoke new when it is an advertisement
                    // ReSharper disable once ConvertToLambdaExpression
                    return !(x.SongInfo.SongMetadata.Artist + x.SongInfo.SongMetadata.Title)
                        .ToLower()
                        .Contains("advertisement");
                })
                .AddBeforeExecution(x =>
                {
                    // Set the stream seeker to the begining
                    x.SongInfo.Stream.Seek(0, SeekOrigin.Begin);
                })
                .SetActionHandler(onSongChanged)
                .AddAfterExecution(_ =>
                {
                    // Reset the memory stream
                    _songInfo.Stream.Clear();
                })
                .WrapAsync()
                .Build();
            
            OnMetadataChanged = ActionEventHandlerBuilder<MetadataChangedEventArg>.New()
                .SetActionHandler(onMetadataChanged)
                .WrapAsync()
                // Trigger song change event
                .AddBeforeExecution(_ =>
                {
                    if (_songInfo.SongMetadata != null)
                    {
                        OnSongChanged(new SongChangedEventArg {
                            SongInfo = new SongInfo
                            {
                                // Pass the song metadata
                                SongMetadata = _songInfo.SongMetadata,

                                // Clone the stream and pass the cloned to event handler
                                Stream = _songInfo.Stream.Clone()
                            }
                        });
                    }
                })
                // Hold on to the metadata
                .AddAfterExecution(x =>
                {
                    // Set the metadata
                    _songInfo.SongMetadata = x.SongMetadata;
                })
                .Build();

            OnStreamUpdate = ActionEventHandlerBuilder<StreamUpdateEventArg>.New()
                .SetActionHandler(onStreamUpdate)
                .WrapAsync()
                // Update the stream
                .AddAfterExecution(x =>
                {
                    _songInfo.Stream.Write(x.SongRawPartial, 0, x.SongRawPartial.Length);
                })
                .Build();
            
            OnStreamStarted = ActionEventHandlerBuilder<StreamStartedEventArg>.New()
                .SetActionHandler(onStreamStarted)
                .WrapAsync()
                // Initialize the buffer
                .AddAfterExecution(_ =>
                {
                    _songInfo = new SongInfo {Stream = new MemoryStream()};
                })
                .Build();

            OnStreamEnded = ActionEventHandlerBuilder<StreamEndedEventArg>.New()
                .SetActionHandler(onStreamEnded)
                .WrapAsync()
                .Build();
        }
    }
}