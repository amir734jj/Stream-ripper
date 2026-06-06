using System;
using System.Threading.Tasks;
using StreamRipper.Models.Events;

namespace StreamRipper.Interfaces
{
    public interface IStreamRipper: IDisposable
    {        
        EventHandler<MetadataChangedEventArg> MetadataChangedHandlers { get; set; }
        
        EventHandler<StreamUpdateEventArg> StreamUpdateEventHandlers { get; set; }
        
        EventHandler<StreamStartedEventArg> StreamStartedEventHandlers { get; set; } 
        
        EventHandler<StreamEndedEventArg> StreamEndedEventHandlers { get; set; }
        
        EventHandler<SongChangedEventArg> SongChangedEventHandlers { get; set; }
        
        EventHandler<StreamFailedEventArg> StreamFailedHandlers { get; set; }
        
        Task<bool> CheckUrlValidAsync();
        
        void Start();
    }
}