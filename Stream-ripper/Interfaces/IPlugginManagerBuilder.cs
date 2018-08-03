using System;
using StreamRipper.Models.Events;

namespace StreamRipper.Interfaces
{
    public interface IPlugginManagerBuilder
    {
        IPlugginManagerBuilder SetOnMetadataChanged(Action<MetadataChangedEventArg> onCurrentSongChanged, 
            Func<MetadataChangedEventArg, bool> filter = null);
        
        IPlugginManagerBuilder SetOnStreamUpdated(Action<StreamUpdateEventArg> onStreamUpdated,
            Func<StreamUpdateEventArg, bool> filter = null);

        IPlugginManagerBuilder SetOnStreamStarted(Action<StreamStartedEventArg> onStreamStarted,
            Func<StreamStartedEventArg, bool> filter = null);

        IPlugginManagerBuilder SetOnStreamEnded(Action<StreamEndedEventArg> onStreamEnded,
            Func<StreamEndedEventArg, bool> filter = null);

        IPluginManager Build();
    }
}