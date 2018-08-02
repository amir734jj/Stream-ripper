using System;
using StreamRipper.Models.Events;

namespace StreamRipper.Interfaces
{
    public interface IPlugginManagerBuilder
    {
        IPlugginManagerBuilder SetOnMetadataChanged(Action<MetadataChangedEventArg> onCurrentSongChanged);
        
        IPlugginManagerBuilder SetOnStreamUpdated(Action<StreamUpdateEventArg> onStreamUpdated);

        IPlugginManagerBuilder SetOnStreamStarted(Action<StreamStartedEventArg> onStreamStarted);

        IPlugginManagerBuilder SetOnStreamEnded(Action<StreamEndedEventArg> onStreamEnded);

        IPluginManager Build();
    }
}