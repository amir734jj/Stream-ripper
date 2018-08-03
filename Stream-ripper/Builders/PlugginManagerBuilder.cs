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
        /// <param name="onMetadataChanged"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnMetadataChanged(Action<MetadataChangedEventArg> onMetadataChanged,
            Func<MetadataChangedEventArg, bool> filter = null) =>
            Run(this, () => _onMetadataChanged = onMetadataChanged,
                () => _onMetadataChanged = FilterAction(_onMetadataChanged, filter?? EmptyFilter<MetadataChangedEventArg>()));

        /// <summary>
        /// Set the on stram update
        /// </summary>
        /// <param name="onStreamUpdated"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnStreamUpdated(Action<StreamUpdateEventArg> onStreamUpdated,
            Func<StreamUpdateEventArg, bool> filter = null) =>
            Run(this, () => _onStreamUpdated = onStreamUpdated,
                () => _onStreamUpdated = FilterAction(_onStreamUpdated, filter?? EmptyFilter<StreamUpdateEventArg>()));

        /// <summary>
        /// Set the on stram started
        /// </summary>
        /// <param name="onStreamStarted"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnStreamStarted(Action<StreamStartedEventArg> onStreamStarted,
            Func<StreamStartedEventArg, bool> filter = null) =>
            Run(this, () => _onStreamStarted = onStreamStarted,
                () => _onStreamStarted = FilterAction(_onStreamStarted, filter?? EmptyFilter<StreamStartedEventArg>()));

        /// <summary>
        /// Set the on stream ended
        /// </summary>
        /// <param name="onStreamEnded"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnStreamEnded(Action<StreamEndedEventArg> onStreamEnded,
            Func<StreamEndedEventArg, bool> filter = null) =>
            Run(this, () => _onStreamEnded = onStreamEnded,
                () => _onStreamEnded = FilterAction(_onStreamEnded, filter?? EmptyFilter<StreamEndedEventArg>()));

        /// <summary>
        /// Set the on song changed
        /// </summary>
        /// <param name="onSongChanged"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IPlugginManagerBuilder SetOnSongChanged(Action<SongChangedEventArg> onSongChanged,
            Func<SongChangedEventArg, bool> filter = null) =>
            Run(this, () => _onSongChanged = onSongChanged,
                () => _onSongChanged = FilterAction(_onSongChanged, filter ?? EmptyFilter<SongChangedEventArg>()));

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