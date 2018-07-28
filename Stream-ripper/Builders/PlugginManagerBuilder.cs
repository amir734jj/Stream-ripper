using System;
using StreamRipper.Models.Events;
using StreamRipper.Pluggins;

namespace StreamRipper.Builders
{
    /// <inheritdoc />
    /// <summary>
    /// Pluggin manager builder
    /// </summary>
    public class PlugginManagerBuilder: BaseBuilder<PlugginManagerBuilder, PluginManager>
    {        
        private Action<MetadataChangedEventArg> _onMetadataChanged;

        private Action<StreamUpdateEventArg> _onStreamUpdated;

        private Action<StreamStartedEventArg> _onStreamStarted;
        
        private Action<SongChangedEventArg> _onSongChanged;

        /// <summary>
        /// Set the on current song changed event
        /// </summary>
        /// <param name="onCurrentSongChanged"></param>
        /// <returns></returns>
        public PlugginManagerBuilder SetOnMetadataChanged(Action<MetadataChangedEventArg> onCurrentSongChanged) =>
            Run(this, () => _onMetadataChanged = onCurrentSongChanged);
        
        /// <summary>
        /// Set the on stram update
        /// </summary>
        /// <param name="onStreamUpdated"></param>
        /// <returns></returns>
        public PlugginManagerBuilder SetOnStreamUpdated(Action<StreamUpdateEventArg> onStreamUpdated) =>
            Run(this, () => _onStreamUpdated = onStreamUpdated);
        
        /// <summary>
        /// Set the on stram started
        /// </summary>
        /// <param name="onStreamStarted"></param>
        /// <returns></returns>
        public PlugginManagerBuilder SetOnStreamStarted(Action<StreamStartedEventArg> onStreamStarted) =>
            Run(this, () => _onStreamStarted = onStreamStarted);
        
        /// <summary>
        /// Set the on song changed
        /// </summary>
        /// <param name="onSongChanged"></param>
        /// <returns></returns>
        public PlugginManagerBuilder SetOnSongChanged(Action<SongChangedEventArg> onSongChanged) =>
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
            _onSongChanged = _onSongChanged ?? EmptyAction<SongChangedEventArg>();
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Build the 
        /// </summary>
        /// <returns></returns>
        public override PluginManager Build() => Run(new PluginManager(_onMetadataChanged, _onStreamUpdated,
            _onStreamStarted, _onSongChanged), BeforeBuild);
    }
}