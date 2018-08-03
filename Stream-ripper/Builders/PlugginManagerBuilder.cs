using System;
using StreamRipper.Interfaces;
using StreamRipper.Models.Events;
using StreamRipper.Pluggins;

namespace StreamRipper.Builders
{
    /// <summary>
    /// Pluggin manager builder
    /// </summary>
    public class PlugginManagerBuilder: BaseBuilder<PlugginManagerBuilder, IPluginManager>, IPlugginManagerBuilder
    {        
        private Action<MetadataChangedEventArg> _onMetadataChanged;

        private Action<StreamUpdateEventArg> _onStreamUpdated;

        private Action<StreamStartedEventArg> _onStreamStarted;
        
        private Action<StreamEndedEventArg> _onStreamEnded;
        
        private Action<SongChangedEventArg> _onSongChanged;

        /// <summary>
        /// Set the on current song changed event
        /// </summary>
        /// <param name="onCurrentSongChanged"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnMetadataChanged(Action<MetadataChangedEventArg> onCurrentSongChanged) =>
            Run(this, () => _onMetadataChanged = onCurrentSongChanged);
        
        /// <summary>
        /// Set the on stram update
        /// </summary>
        /// <param name="onStreamUpdated"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnStreamUpdated(Action<StreamUpdateEventArg> onStreamUpdated) =>
            Run(this, () => _onStreamUpdated = onStreamUpdated);
        
        /// <summary>
        /// Set the on stram started
        /// </summary>
        /// <param name="onStreamStarted"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnStreamStarted(Action<StreamStartedEventArg> onStreamStarted) =>
            Run(this, () => _onStreamStarted = onStreamStarted);
        
        /// <summary>
        /// Set the on stream ended
        /// </summary>
        /// <param name="onStreamEnded"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnStreamEnded(Action<StreamEndedEventArg> onStreamEnded) =>
            Run(this, () => _onStreamEnded = onStreamEnded);
        
        /// <summary>
        /// Set the on song changed
        /// </summary>
        /// <param name="onSongChanged"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnSongChanged(Action<SongChangedEventArg> onSongChanged) =>
            Run(this, () => _onSongChanged = onSongChanged);

        /// <inheritdoc />
        /// <summary>
        /// Before build
        /// </summary>
        /// <returns></returns>
        protected override void BeforeBuild()
        {
            _onMetadataChanged = _onMetadataChanged ?? EmptyAction<MetadataChangedEventArg>();
            _onStreamUpdated = _onStreamUpdated ?? EmptyAction<StreamUpdateEventArg>();
            _onStreamStarted = _onStreamStarted ?? EmptyAction<StreamStartedEventArg>();
            _onStreamEnded = _onStreamEnded ?? EmptyAction<StreamEndedEventArg>();
            _onSongChanged = _onSongChanged ?? EmptyAction<SongChangedEventArg>();
        }
        
        /// <summary>
        /// Build the 
        /// </summary>
        /// <returns></returns>
        public override IPluginManager Build() => Run(new PluginManager(_onMetadataChanged, _onStreamUpdated,
            _onStreamStarted, _onStreamEnded, _onSongChanged), BeforeBuild);
    }
}