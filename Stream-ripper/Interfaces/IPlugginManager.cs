using System;
using StreamRipper.Models.Events;

namespace StreamRipper.Interfaces
{
    public interface IPluginManager
    {
        Action<MetadataChangedEventArg> OnMetadataChanged { get; }

        Action<StreamUpdateEventArg> OnStreamUpdate { get; }
        
        Action<StreamStartedEventArg> OnStreamStarted { get; }
        
        Action<StreamEndedEventArg> OnStreamEnded { get; }
        
        Action<SongChangedEventArg> OnSongChanged { get; }
    }
}